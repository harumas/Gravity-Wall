using Constants;
using UnityEngine;

namespace Module.Gravity
{
    /// <summary>
    /// 触れたプレイヤーの重力を一時的に変更するクラス
    /// </summary>
    public class TemporaryGravityChanger : MonoBehaviour
    {
        [SerializeField,Header("重力の変更倍率")] private float temporaryGravityMultiplier;

        private bool isPlayerEnter;
        private LocalGravity targetGravity;

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag(Tag.Player))
            {
                targetGravity = other.gameObject.GetComponent<LocalGravity>();
                isPlayerEnter = true;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag(Tag.Player))
            {
                isPlayerEnter = false;
            }
        }

        private void FixedUpdate()
        {
            // プレイヤーが触れていたら重力を変更
            if (isPlayerEnter)
            {
                targetGravity.SetMultiplierAtFrame(temporaryGravityMultiplier);
            }
        }
    }
}