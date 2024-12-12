using Constants;
using Module.Player;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    public class GravitySwitchTrigger : MonoBehaviour
    {
        private bool isEnable = true;
        private GravitySwitcher gravitySwitcher;
        
        public void SetEnable(bool isEnable)
        {
            this.isEnable = isEnable;

            if (!isEnable && gravitySwitcher != null)
            {
                gravitySwitcher.Enable();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isEnable)
            {
                return;
            }

            if (other.gameObject.CompareTag(Tag.Player))
            {
                gravitySwitcher = other.GetComponent<GravitySwitcher>();
                gravitySwitcher.Disable();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!isEnable)
            {
                return;
            }

            if (other.gameObject.CompareTag(Tag.Player))
            {
                gravitySwitcher.Enable();
            }
        }
    }
}