using System.Collections;
using System.Collections.Generic;
using Constants;
using Module.Character;
using UnityEngine;

namespace Module.Gimmick
{
    public class GravitySwitchTrigger : MonoBehaviour
    {
        [SerializeField] private bool throughLock;
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
                var gravitySwitcher = other.GetComponent<GravitySwitcher>();

                if (throughLock)
                {
                    gravitySwitcher.LockSwitch = false;
                }

                gravitySwitcher.Disable();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!isEnable) return;

            if (other.gameObject.CompareTag(Tag.Player))
            {
                var gravitySwitcher = other.GetComponent<GravitySwitcher>();

                if (throughLock)
                {
                    gravitySwitcher.LockSwitch = false;
                }

                gravitySwitcher.GetComponent<GravitySwitcher>().Enable();
            }
        }
    }
}