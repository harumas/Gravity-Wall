using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CoreModule.Helper
{
    /// <summary>
    /// 最小値と最大値を定義するクラス
    /// </summary>
    [Serializable]
    public class MinMaxValue
    {
        [SerializeField] private float min;
        [SerializeField] private float max;

        public float Min => min;
        public float Max => max;

        public float GetRandom()
        {
            return Random.Range(min, max);
        }

        public float Clamp(float value)
        {
            return Mathf.Clamp(value, min, max);
        }
    }
}