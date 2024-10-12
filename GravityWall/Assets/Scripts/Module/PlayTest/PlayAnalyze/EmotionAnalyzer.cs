using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Module.PlayTest.EmotionAnalyze
{
    public class EmotionAnalyzer : MonoBehaviour
    {
        [SerializeField] private string folderName;
        [SerializeField] private GameObject stageEmotionPrefab;
        [SerializeField] private Transform stageEmotionParent;

        private readonly Emotion[] emotionTypes = new Emotion[]
        {
            Emotion.Happy,
            Emotion.Sad,
            Emotion.Angry,
            Emotion.Surprised,
            Emotion.Disgusted,
            Emotion.Calm,
            Emotion.Confused,
            Emotion.Fear,
            Emotion.Unknown
        };

        public async void Analyze()
        {
            Debug.Log("Loading emotions...");
            List<PlayData> emotions = await LoadAllEmotionData(Path.Combine("Assets", folderName));
            Dictionary<string, List<PlayData>> groupedEmotions = GroupByStage(emotions);

            foreach ((string stageName, List<PlayData> emotionList) in groupedEmotions)
            {
                var stageEmotionView = Instantiate(stageEmotionPrefab, stageEmotionParent).GetComponent<StageEmotionView>();
                stageEmotionView.SetStageName(stageName);
                
                long averageTicks = (long)Math.Round(emotionList.Select(emotion => emotion.PlayTime).Average());
                stageEmotionView.SetPlayTime(TimeSpan.FromTicks(averageTicks).ToString("mm\\:ss"));

                int rotateCount = (int)Math.Round(emotionList.Select(emotion => emotion.RotateCount).Average());
                stageEmotionView.SetRotateCount(rotateCount);

                int[] totalEmotions = new int[emotionTypes.Length];
                int totalEmotionCount = 0;

                foreach (int emotion in emotionList.SelectMany(data => data.Emotions))
                {
                    totalEmotions[emotion]++;
                    totalEmotionCount += emotionList.Count;
                }

                for (var i = 0; i < totalEmotions.Length; i++)
                {
                    stageEmotionView.SetEmotionRate(emotionTypes[i], totalEmotions[i] / (float)totalEmotionCount * 100f);
                }
            }

            Debug.Log("Completed!");
        }

        private Dictionary<string, List<PlayData>> GroupByStage(List<PlayData> emotions)
        {
            var emotionDictionary = new Dictionary<string, List<PlayData>>();

            foreach (var emotion in emotions)
            {
                if (emotionDictionary.ContainsKey(emotion.StageName))
                {
                    emotionDictionary[emotion.StageName].Add(emotion);
                }
                else
                {
                    emotionDictionary.Add(emotion.StageName, new List<PlayData> { emotion });
                }
            }

            return emotionDictionary;
        }

        private static async UniTask<List<PlayData>> LoadAllEmotionData(string directoryPath)
        {
            var emotionData = new List<PlayData>();

            string[] files = Directory.GetFiles(directoryPath, "*.json");

            foreach (var file in files)
            {
                // ファイルの内容を読み込む
                string jsonContent = await File.ReadAllTextAsync(file);

                // JsonUtilityを使ってデシリアライズ
                PlayData data = JsonUtility.FromJson<PlayData>(jsonContent);
                emotionData.Add(data);
            }

            return emotionData;
        }
    }
}