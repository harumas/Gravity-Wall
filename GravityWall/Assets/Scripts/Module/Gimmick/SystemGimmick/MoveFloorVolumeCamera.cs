using System;
using Cinemachine;
using Constants;
using CoreModule.Helper;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Module.Player;
using R3;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    public class MoveFloorVolumeCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Ease easeType;

        [SerializeField] private SerializableReactiveProperty<bool> isEnabled;
        [SerializeField] private SerializableReactiveProperty<bool> isPlayerRotating;

        [Header("回転のイージング係数")] [SerializeField] private float rotateStep;

        public ReadOnlyReactiveProperty<bool> IsEnabled => isEnabled;
        public Observable<bool> IsRotating => isPlayerRotating;
        public Observable<Quaternion> Rotation => Observable.EveryValueChanged(cameraPivot, target => target.rotation);

        private CinemachineBrain cameraBrain;
        private CameraController cameraController;
        private Transform playerTransform;
        private VerticalAdjuster2d verticalAdjuster;
        private Vector3 currentUpVector;

        private void Start()
        {
            verticalAdjuster = new VerticalAdjuster2d(virtualCamera.transform);
            cameraBrain = Camera.main.GetComponent<CinemachineBrain>();
        }

        private void Update()
        {
            if (!isEnabled.Value || playerTransform == null)
            {
                return;
            }

            EnablePlayerRotate();

            PerformPlayerRotate();
        }

        private void EnablePlayerRotate()
        {
            if (isPlayerRotating.Value)
            {
                return;
            }

            Vector3 direction = verticalAdjuster.GetVerticalDirection(playerTransform.up);

            //90度になったらカメラの向きを変える
            if (direction != Vector3.zero && direction != currentUpVector)
            {
                currentUpVector = direction;
                isPlayerRotating.Value = true;
            }
        }

        public void AssignPlayerTransform(Transform playerTransform, CameraController cameraController)
        {
            this.playerTransform = playerTransform;
            this.cameraController = cameraController;
        }

        public void SetDirection()
        {
            //カメラに一番近い前方ベクトルを取得
            currentUpVector = verticalAdjuster.GetVerticalDirection(playerTransform.up);

            //カメラの前方ベクトルにバーチャルカメラの基準を設定
            cameraPivot.rotation = Quaternion.LookRotation(virtualCamera.transform.forward, currentUpVector);
        }

        private void PerformPlayerRotate()
        {
            if (!isPlayerRotating.Value)
            {
                return;
            }

            Quaternion rotation = Quaternion.FromToRotation(cameraPivot.up, currentUpVector) * cameraPivot.rotation;

            float rotationAngle = Vector3.Angle(cameraPivot.up, currentUpVector);
            bool isLastRotation = PerformRotate(rotation, rotationAngle);
            if (isLastRotation)
            {
                isPlayerRotating.Value = false;
            }
        }

        private bool PerformRotate(Quaternion targetRotation, float rotationAngle)
        {
            bool isLastRotation;
            Quaternion nextRotation;

            //カメラとプレイヤーの角度の差がほぼなくなったら終了
            if (Mathf.Abs(rotationAngle) < 0.1f)
            {
                nextRotation = targetRotation;
                isLastRotation = true;
            }
            else
            {
                //イージング関数を噛ませる
                float t = EaseUtility.Evaluate(easeType, rotationAngle, rotateStep);
                nextRotation = Quaternion.Slerp(cameraPivot.rotation, targetRotation, t * Time.deltaTime);
                isLastRotation = false;
            }


            cameraPivot.rotation = nextRotation;

            return isLastRotation;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                Enable().Forget();
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (playerTransform == null)
            {
                return;
            }

            if (other.CompareTag(Tag.Player))
            {
                Disable();
            }
        }

        private async UniTaskVoid Enable()
        {
            await UniTask.WaitUntil(() => playerTransform != null);

            SetDirection();
            virtualCamera.Priority = 11;
            isEnabled.Value = true;
        }

        private void Disable()
        {
            if (cameraController != null)
            {
                cameraController.SetCameraRotation(cameraPivot.rotation);
            }

            isEnabled.Value = false;
            virtualCamera.Priority = 0;
        }

        private void OnDisable()
        {
            Disable();
        }
    }
}