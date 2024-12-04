using Constants;
using Module.Player;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    /// <summary>
    /// プレイヤーが入ったら重力変更を無効化するトリガー
    /// </summary>
    public class GravitySwitchTrigger : MonoBehaviour
    {
        private bool isEnable = true;
        private GravitySwitcher gravitySwitcher;
        
        public void SetEnable(bool isEnable)
        {
            this.isEnable = isEnable;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isEnable)
            {
                return;
            }

            // プレイヤーが入ったら無効化
            if (other.gameObject.CompareTag(Tag.Player))
            {
                // 初めて入る際はGravitySwitcherをキャッシュする
                gravitySwitcher ??= other.GetComponent<GravitySwitcher>();
                gravitySwitcher.Disable();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!isEnable)
            {
                return;
            }

            // プレイヤーが出たら有効化
            if (other.gameObject.CompareTag(Tag.Player))
            {
                gravitySwitcher.Enable();
            }
        }
    }
}