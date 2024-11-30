using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Module.InputModule
{
    [CreateAssetMenu(fileName = "VibrationParameter", menuName = "VibrationParameter")]
    public class VibrationParameter : ScriptableObject
    {
        [SerializeField] private float deathDuration;
        [SerializeField] private float deathSpeed;
        [SerializeField] private float rotateDuration;
        [SerializeField] private float rotateVibrationInterval;
        [SerializeField] private AnimationCurve angleVibrationCurve;

        public float DeathDuration => deathDuration;
        public float DeathSpeed => deathSpeed;

        public float RotateDuration => rotateDuration;
        public float RotateVibrationInterval => rotateVibrationInterval;

        public float EvaluateAngleVibration(float angle)
        {
            return angleVibrationCurve.Evaluate(angle);
        }
    }
}