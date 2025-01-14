using System;
using UnityEngine;

namespace Module.PlayAnalyze
{
    /// <summary>
    /// 1ステージあたりのプレイデータ
    /// </summary>
    [Serializable]
    public class PlayData
    {
        public long PlayTime => playTime;
        public string StageName => stageName;
        public int[] Emotions => emotions;
        public Vector3[] Positions => positions;
        public int RotateCount => rotateCount;
        public int DeathCount => deathCount;

        [SerializeField] private long playTime;
        [SerializeField] private string stageName;
        [SerializeField] private int[] emotions;
        [SerializeField] private Vector3[] positions;
        [SerializeField] private int rotateCount;
        [SerializeField] private int deathCount;

        public PlayData(long playTime, string stageName, int[] emotions, Vector3[] positions, int rotateCount,int deathCount)
        {
            this.playTime = playTime;
            this.stageName = stageName;
            this.emotions = emotions;
            this.positions = positions;
            this.rotateCount = rotateCount;
            this.deathCount = deathCount;
        }
    }
}