using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Amazon;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.Runtime;
using Cysharp.Threading.Tasks;
using Module.Character;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Module.PlayTest.EmotionAnalyze
{
    public class EmotionCapture
    {
        private readonly WebCamTexture webCamTexture;
        private readonly Texture2D snapshot;
        private readonly AmazonRekognitionClient rekognitionClient;

        public EmotionCapture(string accessKey, string secretKey)
        {
            // AWSの認証情報とクライアントの設定
            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            rekognitionClient = new AmazonRekognitionClient(credentials, RegionEndpoint.USEast1);

            // Webカメラのデバイスを取得
            WebCamDevice[] devices = WebCamTexture.devices;
            if (devices.Length > 0)
            {
                webCamTexture = new WebCamTexture(devices[0].name);
                webCamTexture.Play();
            }
            else
            {
                Debug.LogError("Webカメラが見つかりません。");
            }


            snapshot = new Texture2D(webCamTexture.width, webCamTexture.height);
        }

        // ボタンなどで呼び出して現在のフレームをキャプチャし、Amazon Rekognitionで解析
        public async UniTaskVoid CaptureAsync(CancellationToken cancellationToken, Action<EmotionName> action)
        {
            // 現在のWebCamTextureのフレームを取得
            snapshot.SetPixels(webCamTexture.GetPixels());
            snapshot.Apply();

            // 画像データをPNG形式にエンコード
            byte[] imageBytes = snapshot.EncodeToPNG();

            // Amazon Rekognitionに送信するためのバイトストリームに変換
            MemoryStream imageStream = new MemoryStream(imageBytes);

            // Amazon Rekognitionに感情分析をリクエスト
            var request = new DetectFacesRequest
            {
                Image = new Amazon.Rekognition.Model.Image
                {
                    Bytes = imageStream
                },
                Attributes = new List<string> { "ALL" } // 感情などのすべての属性を取得
            };

            // Amazon Rekognitionに非同期でリクエストを送信
            try
            {
                var response = await rekognitionClient.DetectFacesAsync(request, cancellationToken);
                if (response.HttpStatusCode == HttpStatusCode.OK)
                {
                    EmotionName emotionName = response.FaceDetails.Count > 0 ? response.FaceDetails[0].Emotions[0].Type : EmotionName.UNKNOWN;
                    action?.Invoke(emotionName);
                    return;
                }

                Debug.LogError($"Error detecting faces: {response.HttpStatusCode}");
                action?.Invoke(EmotionName.UNKNOWN);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void Close()
        {
            if (webCamTexture != null)
            {
                webCamTexture.Stop();
            }
        }
    }
}