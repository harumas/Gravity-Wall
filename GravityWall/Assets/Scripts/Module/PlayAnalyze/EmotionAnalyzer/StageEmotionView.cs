using TMPro;
using UnityEngine;

namespace Module.PlayAnalyze.EmotionAnalyzer
{
    public enum Emotion
    {
        Happy,
        Sad,
        Angry,
        Surprised,
        Disgusted,
        Calm,
        Confused,
        Fear,
        Unknown
    }

    /// <summary>
    /// ステージの感情分析結果のパネルを表示するクラス
    /// </summary>
    public class StageEmotionView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI stageNameText;
        [SerializeField] private TextMeshProUGUI playTimeText;
        [SerializeField] private TextMeshProUGUI rotateCountText;
        [SerializeField] private TextMeshProUGUI happyText;
        [SerializeField] private TextMeshProUGUI sadText;
        [SerializeField] private TextMeshProUGUI angryText;
        [SerializeField] private TextMeshProUGUI surprisedText;
        [SerializeField] private TextMeshProUGUI disgustedText;
        [SerializeField] private TextMeshProUGUI calmText;
        [SerializeField] private TextMeshProUGUI confusedText;

        public void SetStageName(string stageName)
        {
            stageNameText.text = stageName;
        }

        public void SetPlayTime(string playTime)
        {
            playTimeText.text = $"PlayTime\nAve. {playTime}";
        }
        
        public void SetRotateCount(int rotateCount)
        {
            rotateCountText.text = $"RotateCount\nAve. {rotateCount}";
        }

        public void SetEmotionRate(Emotion emotion, float rate)
        {
            string text = $"{emotion.ToString()}: {Mathf.RoundToInt(rate)}%";
            switch (emotion)
            {
                case Emotion.Happy:
                    happyText.text = text;
                    break;
                case Emotion.Sad:
                    sadText.text = text;
                    break;
                case Emotion.Angry:
                    angryText.text = text;
                    break;
                case Emotion.Surprised:
                    surprisedText.text = text;
                    break;
                case Emotion.Disgusted:
                    disgustedText.text = text;
                    break;
                case Emotion.Calm:
                    calmText.text = text;
                    break;
                case Emotion.Confused:
                    confusedText.text = text;
                    break;

                case Emotion.Fear:
                case Emotion.Unknown:
                    break;
            }
        }
    }
}