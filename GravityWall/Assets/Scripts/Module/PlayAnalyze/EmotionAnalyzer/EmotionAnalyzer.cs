using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Module.PlayAnalyze.EmotionAnalyzer
{
    /// <summary>
    /// 感情分析結果を集計するクラス
    /// </summary>
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

            // ステージ毎のプレイデータを取得する
            List<PlayData> emotions = await LoadAllEmotionData(Path.Combine("Assets", folderName));
            Dictionary<string, List<PlayData>> groupedEmotions = GroupByStage(emotions);

            // ステージ毎に感情分析結果のパネルを作成
            foreach ((string stageName, List<PlayData> emotionList) in groupedEmotions)
            {
                CreateStageEmotionView(stageName, emotionList);
            }

            Debug.Log("Completed!");
        }

        private void CreateStageEmotionView(string stageName, List<PlayData> emotionList)
        {
            // ステージ名
            var stageEmotionView = Instantiate(stageEmotionPrefab, stageEmotionParent).GetComponent<StageEmotionView>();
            stageEmotionView.SetStageName(stageName);

            // プレイ時間の平均
            long averageTicks = (long)Math.Round(emotionList.Select(emotion => emotion.PlayTime).Average());
            stageEmotionView.SetPlayTime(TimeSpan.FromTicks(averageTicks).ToString("mm\\:ss"));

            // 回転数の平均
            int rotateCount = (int)Math.Round(emotionList.Select(emotion => emotion.RotateCount).Average());
            stageEmotionView.SetRotateCount(rotateCount);

            // 感情の集計
            int[] totalEmotions = new int[emotionTypes.Length];
            int totalEmotionCount = 0;

            foreach (int emotion in emotionList.SelectMany(data => data.Emotions))
            {
                totalEmotions[emotion]++;
                totalEmotionCount += emotionList.Count;
            }

            // 感情の割合を表示
            for (var i = 0; i < totalEmotions.Length; i++)
            {
                stageEmotionView.SetEmotionRate(emotionTypes[i], totalEmotions[i] / (float)totalEmotionCount * 100f);
            }
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