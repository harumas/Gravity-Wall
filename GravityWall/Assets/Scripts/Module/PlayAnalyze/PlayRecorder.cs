using System;
using System.Collections.Generic;
using System.Threading;
using Amazon.Rekognition;
using CoreModule.Save;
using Cysharp.Threading.Tasks;
using Module.PlayAnalyze.EmotionAnalyzer;
using Module.Player;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Module.Player.PlayerController;

namespace Module.PlayAnalyze
{
    /// <summary>
    /// プレイ情報を収集するクラス
    /// </summary>
    public class PlayRecorder : MonoBehaviour
    {
        [SerializeField, Header("収集する間隔")] private float recordInterval;
        [SerializeField, Header("AWSのアクセスキー")] private string accessKey;
        [SerializeField, Header("AWSのシークレットキー")] private string secretKey;
        [SerializeField, Header("カメラ撮影を有効化するか")] private bool enableCameraCapture;

        private Dictionary<EmotionName, int> emotionIndexMap;
        private EmotionCapture emotionCapture;
        private CancellationTokenSource cancellationTokenSource;
        private PlayerController playerController;
        private Action onPlayerRotate;
        private Action<DeathType> onPlayerDeath;

        private void Start()
        {
            if (enableCameraCapture)
            {
                emotionCapture = new EmotionCapture(accessKey, secretKey);
                emotionIndexMap = new Dictionary<EmotionName, int>()
                {
                    { EmotionName.HAPPY, 0 },
                    { EmotionName.SAD, 1 },
                    { EmotionName.ANGRY, 2 },
                    { EmotionName.SURPRISED, 3 },
                    { EmotionName.DISGUSTED, 4 },
                    { EmotionName.CALM, 5 },
                    { EmotionName.CONFUSED, 6 },
                    { EmotionName.FEAR, 7 },
                    { EmotionName.UNKNOWN, 8 },
                };
            }

            StartRecording();
        }

        /// <summary>
        /// プレイ情報の収集を開始します
        /// </summary>
        public async void StartRecording()
        {
            Debug.Log("プレイ情報の記録を開始しました");
            cancellationTokenSource = new CancellationTokenSource();

            // キャプチャー開始
            DateTime startTime = DateTime.Now;
            string stageName = SceneManager.GetActiveScene().name;
            List<int> emotions = new List<int>(1024);
            List<Vector3> positions = new List<Vector3>(1024);

            int rotateCount = 0;
            int deathCount = 0;

            // プレイヤーの回転数と死亡数を収集
            onPlayerRotate += () => rotateCount++;
            onPlayerDeath += (value) => { if (value != DeathType.None) deathCount++; };

            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                // 表情から感情を収集
                CaptureEmotion(emotions);

                // プレイヤー座標を収集
                CapturePlayerPosition(positions);

                await UniTask.Delay(TimeSpan.FromSeconds(recordInterval));
            }

            DateTime endTime = DateTime.Now;

            // プレイ情報を保存
            string fileName = $"PlayData_{startTime.ToString("MMddmmss")}";
            long playTime = (endTime - startTime).Ticks;
            PlayData playData = new PlayData(playTime, stageName, emotions.ToArray(), positions.ToArray(), rotateCount, deathCount);

            await SaveUtility.Save(playData, fileName, true);

            Debug.Log("プレイ情報を保存しました");
        }

        /// <summary>
        /// プレイ情報の収集を停止します
        /// </summary>
        public void StopRecording()
        {
            cancellationTokenSource?.Cancel();
        }

        private void CaptureEmotion(List<int> emotions)
        {
            if (!enableCameraCapture)
            {
                return;
            }

            // 非同期で表情をキャプチャー
            emotionCapture.CaptureAsync(cancellationTokenSource.Token, emotion =>
            {
                emotions.Add(emotionIndexMap[emotion]);
                Debug.Log($"Emotion: {emotion}");
            }).Forget();
        }

        private void CapturePlayerPosition(List<Vector3> positions)
        {
            Vector3 playerPosition;
            if (playerController != null)
            {
                playerPosition = playerController.transform.position;
            }
            else
            {
                playerController = GetPlayerController();
                playerPosition = playerController ? playerController.transform.position : Vector3.negativeInfinity;
            }

            positions.Add(playerPosition);
        }

        private PlayerController GetPlayerController()
        {
            PlayerController controller = FindObjectOfType<PlayerController>();

            if (controller != null)
            {
                // 回転イベント
                controller.IsRotating.Subscribe(value =>
                {
                    if (value)
                    {
                        onPlayerRotate?.Invoke();
                    }
                }).AddTo(cancellationTokenSource.Token);

                // 死亡イベント
                controller.IsDeath.Subscribe(value =>
                {
                    if (value != DeathType.None)
                    {
                        onPlayerDeath?.Invoke(value);
                    }
                }).AddTo(cancellationTokenSource.Token);
            }

            return controller;
        }

        private void OnDestroy()
        {
            StopRecording();
            emotionCapture?.Close();
        }
    }
}