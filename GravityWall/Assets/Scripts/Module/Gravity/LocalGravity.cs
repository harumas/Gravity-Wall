using UnityEngine;

namespace Module.Gravity
{
    /// <summary>
    /// Rigidbodyに独自の倍率の重力を提供するクラス
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class LocalGravity : MonoBehaviour
    {
        [SerializeField, Header("重力の倍率")] private float multiplier = 1f;
        [SerializeField, Header("オブジェクトに適用する重力タイプ")] private WorldGravity.Type gravityType;
        private Rigidbody rigBody;

        private bool isExternalMultiplierFrame;
        private float frameMultiplier;

        private void Awake()
        {
            rigBody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (WorldGravity.Instance.IsEnable(gravityType))
            {
                // 外部から倍率を設定された場合はそちらを利用する
                float currentMultiplier = isExternalMultiplierFrame ? frameMultiplier : multiplier;
                isExternalMultiplierFrame = false;

                // 重力方向に加速
                rigBody.AddForce(WorldGravity.Instance.Gravity * currentMultiplier, ForceMode.Acceleration);
            }
            else
            {
                rigBody.velocity = Vector3.zero;
            }
        }

        public void SetMultiplierAtFrame(float multiplier)
        {
            frameMultiplier = multiplier;
            isExternalMultiplierFrame = true;
        }

        public void Enable()
        {
            enabled = true;
        }

        public void Disable()
        {
            enabled = false;
            rigBody.velocity = Vector3.zero;
        }
    }
}