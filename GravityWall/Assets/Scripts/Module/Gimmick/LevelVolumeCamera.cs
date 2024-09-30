using System;
using Cinemachine;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using Module.Character;
using Module.Gimmick.LevelMask;
using R3;
using UnityEngine;

namespace Module.Gimmick
{
    public class LevelVolumeCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Ease easeType;

        [Header("回転のイージング係数")]
        [SerializeField]
        private float rotateStep;

        [SerializeField] private bool isEnabled;
        [SerializeField] private bool isPlayerRotating;
        [SerializeField] private bool isInputRotating;

        private MaskVolume maskVolume;
        private Transform playerTransform;
        private CameraController cameraController;
        private PlayerController playerController;
        private Vector3 currentUpVector;

        public void AssignPlayerTransform(Transform playerTransform, CameraController cameraController, PlayerController playerController)
        {
            this.playerTransform = playerTransform;
            this.cameraController = cameraController;
            this.playerController = playerController;
        }

        private readonly Vector3[] directions =
        {
            Vector3.up,
            Vector3.down,
            Vector3.right,
            Vector3.left,
            Vector3.forward,
            Vector3.back,
        };

        private Vector3 GetNearestDirection(Vector3 origin)
        {
            const float error = 0.01f;

            foreach (Vector3 direction in directions)
            {
                if (1f - Vector3.Dot(direction, origin) < error)
                {
                    return direction;
                }
            }

            return Vector3.zero;
        }

        private void Update()
        {
            if (!isEnabled || playerTransform == null)
            {
                return;
            }

            if (isInputRotating)
            {
                PerformAdditionalRotate();
            }
            else
            {
                RotatePlayerCamera();
            }
        }

        private void RotatePlayerCamera()
        {
            Vector3 direction = GetNearestDirection(playerTransform.up);

            //90度になったらカメラの向きを変える
            if (isPlayerRotating || direction != currentUpVector)
            {
                if (!isPlayerRotating)
                {
                    currentUpVector = direction;
                    isPlayerRotating = true;
                }

                float rotationAngle = Vector3.Angle(cameraPivot.up, currentUpVector);
                Quaternion rotation = Quaternion.FromToRotation(cameraPivot.up, currentUpVector) * cameraPivot.rotation;

                bool isLastRotation = PerformRotate(rotation, rotationAngle);
                if (isLastRotation)
                {
                    isPlayerRotating = false;
                }
            }
        }


        private bool PerformRotate(Quaternion targetRotation, float rotationAngle)
        {
            bool isLastRotation;
            Quaternion nextRotation;

            if (Mathf.Abs(rotationAngle) < 0.1f)
            {
                nextRotation = targetRotation;
                playerController.IsRotationLocked = false;
                isLastRotation = true;
            }
            else
            {
                float t = Evaluate(easeType, rotationAngle, rotateStep);
                nextRotation = Quaternion.Slerp(cameraPivot.rotation, targetRotation, t * Time.deltaTime);
                playerController.IsRotationLocked = true;
                isLastRotation = false;
            }

            cameraPivot.rotation = nextRotation;
            cameraController.SetCameraRotation(nextRotation);

            return isLastRotation;
        }

        private Quaternion additionalRotation;

        public void EnableAdditionalRotate(int value)
        {
            if (isPlayerRotating || isInputRotating)
            {
                return;
            }

            additionalRotation = Quaternion.AngleAxis(value * 90f, cameraPivot.up) * cameraPivot.rotation;
            isInputRotating = true;
        }

        private void PerformAdditionalRotate()
        {
            float rotationAngle = Quaternion.Angle(cameraPivot.rotation, additionalRotation);
            bool isLastRotation = PerformRotate(additionalRotation, rotationAngle);
            if (isLastRotation)
            {
                isInputRotating = false;
            }
        }

        private float Evaluate(Ease easeType, float angle, float step)
        {
            if (easeType == Ease.Unset)
            {
                return step;
            }

            return EaseManager.Evaluate(easeType, null, step, angle, 1f, 1f);
        }

        private void Awake()
        {
            maskVolume = transform.parent.GetComponent<MaskVolume>();

            if (maskVolume == null)
            {
                Debug.LogError("MaskVolumeが見つかりませんでした");
                return;
            }

            AssignVolumeEvent();
        }

        private void AssignVolumeEvent()
        {
            maskVolume.IsEnable.Subscribe(isEnable =>
                {
                    if (isEnable)
                    {
                        Enable();
                    }
                    else
                    {
                        Disable();
                    }
                })
                .AddTo(this);
        }

        private void Enable()
        {
            isEnabled = true;
            virtualCamera.Priority = 11;

            if (cameraController != null)
            {
                cameraController.SetFreeCamera(false);

                currentUpVector = GetNearestDirection(playerTransform.up);
                cameraController.SetCameraRotation(cameraPivot.rotation);
            }
        }

        private void Disable()
        {
            isEnabled = false;
            virtualCamera.Priority = 0;

            if (cameraController != null)
            {
                cameraController.SetFreeCamera(true);
            }
        }
    }
}