using System.Collections;
using System.Collections.Generic;
using Constants;
using Module.Character;
using UnityEngine;

namespace Module.Gimmick
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