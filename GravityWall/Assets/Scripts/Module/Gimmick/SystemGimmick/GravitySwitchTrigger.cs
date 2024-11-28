using Constants;
using Module.Player;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    public class GravitySwitchTrigger : MonoBehaviour
    {
        private bool isEnable = true;
        public void SetEnable(bool isEnable)
        {
            this.isEnable = isEnable;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isEnable) return;

            if (other.gameObject.CompareTag(Tag.Player))
            {
                other.GetComponent<GravitySwitcher>().Disable();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!isEnable) return;

            if (other.gameObject.CompareTag(Tag.Player))
            {
                other.GetComponent<GravitySwitcher>().Enable();
            }
        }
    }
}