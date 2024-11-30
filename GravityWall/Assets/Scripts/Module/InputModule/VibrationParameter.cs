using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Module.InputModule
{
    [Serializable]
    public class Vibration
    {
        [SerializeField, Header("持続時間")] private float duration;
        [SerializeField, Header("右側の速度")] private float rightSpeed;
        [SerializeField, Header("左側の速度")] private float leftSpeed;

        public float Duration => duration;
        public float RightSpeed => rightSpeed;
        public float LeftSpeed => leftSpeed;
    }

    [CreateAssetMenu(fileName = "VibrationParameter", menuName = "VibrationParameter")]
    public class VibrationParameter : ScriptableObject
    {
        [SerializeField] private Vibration jump;
        [SerializeField] private Vibration death;
        [SerializeField] private Vibration rotate;
        [SerializeField] private float vibrationInterval = 0.1f;

        public Vibration Jump => jump;
        public Vibration Death => death;
        public Vibration Rotate => rotate;

        public float VibrationInterval => vibrationInterval;
    }
}