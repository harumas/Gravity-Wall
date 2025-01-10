using System;
using Constants;
using Module.Player;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    public class GravitySwitchTrigger : MonoBehaviour
    {
        private bool isEnable = true;
        private bool isPlayerEnter = false;
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
                isPlayerEnter = true;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!isEnable || isPlayerEnter)
            {
                return;
            }

            if (other.gameObject.CompareTag(Tag.Player))
            {
                gravitySwitcher = other.GetComponent<GravitySwitcher>();
                gravitySwitcher.Disable();
                isPlayerEnter = true;
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
                isPlayerEnter = false;
            }
        }

        private void OnDisable()
        {
            if (isPlayerEnter)
            {
                gravitySwitcher.Enable();
                isPlayerEnter = false;
            }
        }
    }
}