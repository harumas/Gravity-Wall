using System;
using Constants;
using Module.Player;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    public class InGameEventPlayerTrap : MonoBehaviour
    {
        [SerializeField] private Transform target;

        public event Action OnTrapped;

        private PlayerController playerController;
        private GravitySwitcher gravitySwitcher;
        private PlayerTargetSyncer playerTargetSyncer;
        private CameraController cameraController;
        private bool isEnable = false;

        private void OnTriggerEnter(Collider other)
        {
            if (isEnable)
            {
                return;
            }

            GameObject obj = other.gameObject;

            if (obj.CompareTag(Tag.Player))
            {
                playerController = obj.GetComponent<PlayerController>();
                gravitySwitcher = obj.GetComponent<GravitySwitcher>();
                playerTargetSyncer = obj.GetComponentInChildren<PlayerTargetSyncer>();
                cameraController = obj.GetComponentInChildren<CameraController>();

                Enable();

                other.transform.position = target.position;
                isEnable = true;

                OnTrapped?.Invoke();
            }
        }

        private void Enable()
        {
            cameraController.SetCameraRotation(target.rotation);
            cameraController.SetFreeCamera(false);

            playerController.Refresh();
            playerController.Lock();
            gravitySwitcher.Disable();
            playerTargetSyncer.Lock();

            playerController.HoldLock = true;
            playerTargetSyncer.HoldLock = true;
        }

        public void Disable(bool isGravityEnable)
        {
            playerController.HoldLock = false;
            playerTargetSyncer.HoldLock = false;

            cameraController.SetFreeCamera(true);
            playerController.Unlock();
            playerTargetSyncer.Unlock();
            if (isGravityEnable)
            {
                gravitySwitcher.Enable();
            }
        }
    }
}