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
        private Animator playerAnimator;
        private bool isEnable = false;
        private readonly string isInstallAnimationName = "IsInstall";

        private void OnTriggerEnter(Collider other)
        {
            if (isEnable)
            {
                return;
            }

            GameObject obj = other.gameObject;

            if (!obj.CompareTag(Tag.Player))
            {
                return;
            }

            if (playerController == null)
            {
                CacheComponents();
            }

            if (!IsValidDirection(playerController.transform.up))
            {
                return;
            }

            obj.transform.position = target.position;
            playerTargetSyncer.SetRotation(target.rotation);

            Enable();

            isEnable = true;

            OnTrapped?.Invoke();
        }

        private void CacheComponents()
        {
            playerController = GetComponent<PlayerController>();
            gravitySwitcher = GetComponent<GravitySwitcher>();
            playerTargetSyncer = GetComponentInChildren<PlayerTargetSyncer>();
            cameraController = GetComponentInChildren<CameraController>();
            playerAnimator = GetComponentInChildren<Animator>();
        }

        private bool IsValidDirection(Vector3 up)
        {
            const float tolerance = 0.01f;
            return (up - target.up).sqrMagnitude < tolerance * tolerance;
        }

        private void Enable()
        {
            cameraController.SetFreeCamera(false);
            cameraController.SetCameraRotation(target.rotation);

            const RigidbodyConstraints freezeXZ = RigidbodyConstraints.FreezePositionX |
                                                  RigidbodyConstraints.FreezePositionZ |
                                                  RigidbodyConstraints.FreezeRotation;

            playerController.Refresh();
            playerController.Lock(freezeXZ);
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

        public void PlayPlayerInstallAnimation(bool isInstall)
        {
            playerAnimator.SetBool(isInstallAnimationName, isInstall);
        }
    }
}