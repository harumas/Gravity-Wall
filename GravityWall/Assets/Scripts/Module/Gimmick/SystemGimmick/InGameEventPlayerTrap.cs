using System;
using Constants;
using Module.Player;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    public class InGameEventPlayerTrap : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private RigidbodyConstraints[] constraints;

        public event Action OnTrapped;

        private PlayerController playerController;
        private GravitySwitcher gravitySwitcher;
        private PlayerTargetSyncer playerTargetSyncer;
        private CameraController cameraController;
        private Animator playerAnimator;
        private RigidbodyConstraints constraintsFlag;
        private bool isEnable = false;
        private readonly string isInstallAnimationName = "IsInstall";

        private void Awake()
        {
            foreach (RigidbodyConstraints constraint in constraints)
            {
                constraintsFlag |= constraint;
            }
        }

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
                CacheComponents(obj);
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

        private void CacheComponents(GameObject gameObject)
        {
            playerController = gameObject.GetComponent<PlayerController>();
            gravitySwitcher = gameObject.GetComponent<GravitySwitcher>();
            playerTargetSyncer = gameObject.GetComponentInChildren<PlayerTargetSyncer>();
            cameraController = gameObject.GetComponentInChildren<CameraController>();
            playerAnimator = gameObject.GetComponentInChildren<Animator>();
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

            playerController.Lock(constraintsFlag, true);
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