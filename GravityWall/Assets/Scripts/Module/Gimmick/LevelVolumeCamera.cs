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
        [SerializeField] private bool isRotating;

        private MaskVolume maskVolume;
        private Transform playerTransform;
        private CameraController cameraController;
        private Vector3 currentUpVector;

        public void AssignPlayerTransform(Transform playerTransform, CameraController cameraController)
        {
            this.playerTransform = playerTransform;
            this.cameraController = cameraController;

            currentUpVector = playerTransform.up;
            cameraController.SetCameraRotation(cameraPivot.rotation);
        }

        private void Update()
        {
            if (!isEnabled || playerTransform == null)
            {
                return;
            }

            float angle = Vector3.Angle(currentUpVector, playerTransform.up);
            bool canRotate = Mathf.Abs(angle - 90f) < 0.1f;

            Debug.Log(angle);

            //90度になったらカメラの向きを変える
            if (isRotating || canRotate)
            {
                Quaternion rotation = Quaternion.FromToRotation(cameraPivot.up, playerTransform.up) * cameraPivot.rotation;
                Quaternion nextRotation;

                if (Mathf.Abs(angle) < 0.1f)
                {
                    nextRotation = rotation;
                    currentUpVector = playerTransform.up;
                    Debug.Log("false!!!");
                    isRotating = false;
                }
                else
                {
                    float t = Evaluate(easeType, angle, rotateStep);
                    nextRotation = Quaternion.Slerp(cameraPivot.rotation, rotation, t * Time.deltaTime);
                    isRotating = true;
                }

                cameraPivot.rotation = nextRotation;
                cameraController.SetCameraRotation(nextRotation);
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
            }).AddTo(this);
        }

        private void Enable()
        {
            isEnabled = true;
            virtualCamera.Priority = 11;

            if (cameraController != null)
            {
                cameraController.SetFreeCamera(false);
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