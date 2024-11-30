using UnityEngine;

namespace Module.InputModule
{
    /// <summary>
    /// バイブレーションのパラメータを保持するクラス
    /// </summary>
    [CreateAssetMenu(fileName = "VibrationParameter", menuName = "VibrationParameter")]
    public class VibrationParameter : ScriptableObject
    {
        [Header("死亡イベント")]
        [SerializeField, Header("バイブレーション時間")]
        private float deathDuration;

        [SerializeField, Header("バイブレーション強度"), Range(0f, 1f)]
        private float deathSpeed;

        [Header("回転イベント")]
        [SerializeField, Header("バイブレーション時間")]
        private float rotateDuration;

        [SerializeField, Header("バイブレーションの作動インターバル")] private float rotateVibrationInterval;
        [SerializeField, Header("角度とバイブレーション強度のカーブ")] private AnimationCurve angleVibrationCurve;

        public float DeathDuration => deathDuration;
        public float DeathSpeed => deathSpeed;

        public float RotateDuration => rotateDuration;
        public float RotateVibrationInterval => rotateVibrationInterval;

        /// <summary>
        /// 角度に対応するバイブレーションの強さを取得します
        /// </summary>
        public float EvaluateAngleVibration(float angle)
        {
            return angleVibrationCurve.Evaluate(angle);
        }
    }
}