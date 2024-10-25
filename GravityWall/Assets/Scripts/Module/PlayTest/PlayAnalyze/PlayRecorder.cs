using System;
using System.Collections.Generic;
using System.Threading;
using Amazon.Rekognition;
using CoreModule.Save;
using Cysharp.Threading.Tasks;
using Module.Character;
using Module.PlayTest.EmotionAnalyze;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Module.PlayTest.PlayAnalyze
{
    public class PlayRecorder : MonoBehaviour
    {
        [SerializeField] private float recordInterval;
        [SerializeField] private string accessKey;
        [SerializeField] private string secretKey;
        [SerializeField] private bool enableCameraCapture;

        private Dictionary<EmotionName, int> emotionIndexMap;
        private EmotionCapture emotionCapture;
        private CancellationTokenSource cancellationTokenSource;
        private PlayerController playerController;
        private Action onPlayerRotate;
        private Action onPlayerDeath;

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

            onPlayerRotate += () => rotateCount++;
            onPlayerDeath += () => deathCount++;

            while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                emotionCapture?.CaptureAsync(cancellationTokenSource.Token, emotion =>
                {
                    emotions.Add(emotionIndexMap[emotion]);
                    Debug.Log($"Emotion: {emotion}");
                }).Forget();

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

                await UniTask.Delay(TimeSpan.FromSeconds(recordInterval));
            }

            DateTime endTime = DateTime.Now;

            string fileName = $"PlayData_{startTime.ToString("MMddmmss")}";
            await SaveUtility.Save(
                new PlayData((endTime - startTime).Ticks, stageName, emotions.ToArray(), positions.ToArray(), rotateCount, deathCount),
                fileName, true);
            
            Debug.Log("プレイ情報を保存しました");
        }

        private PlayerController GetPlayerController()
        {
            PlayerController controller = FindObjectOfType<PlayerController>();

            if (controller != null)
            {
                controller.State.IsRotating.Subscribe(value =>
                {
                    if (value)
                    {
                        onPlayerRotate?.Invoke();
                    }
                }).AddTo(cancellationTokenSource.Token);
                
                controller.State.IsDeath.Subscribe(value =>
                {
                    if (value)
                    {
                        onPlayerDeath?.Invoke();
                    }
                }).AddTo(cancellationTokenSource.Token);
            }

            return controller;
        }

        public void StopRecording()
        {
            cancellationTokenSource?.Cancel();
        }

        private void OnDestroy()
        {
            StopRecording();
            emotionCapture?.Close();
        }
    }
}