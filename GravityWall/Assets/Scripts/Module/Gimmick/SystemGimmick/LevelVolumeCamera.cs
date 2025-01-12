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
    /// <summary>
    /// レベルボリュームを俯瞰するカメラ
    /// </summary>
    public class LevelVolumeCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Ease easeType;

        [Header("回転のイージング係数")] [SerializeField] private float rotateStep;

        [SerializeField] private SerializableReactiveProperty<bool> isEnabled;
        [SerializeField] private SerializableReactiveProperty<bool> isPlayerRotating;
        [SerializeField] private SerializableReactiveProperty<bool> isInputRotating;

        public ReadOnlyReactiveProperty<bool> IsEnabled => isEnabled;
        public Observable<bool> IsRotating => Observable.Merge(isPlayerRotating, isInputRotating);
        public Observable<Quaternion> Rotation => Observable.EveryValueChanged(cameraPivot, target => target.rotation);

        private CinemachineBrain cameraBrain;
        private CameraController cameraController;
        private VerticalAdjuster verticalAdjuster;
        private Transform playerTransform;
        private Vector3 currentUpVector;

        public void AssignPlayerTransform(Transform playerTransform, CameraController cameraController)
        {
            this.playerTransform = playerTransform;
            this.cameraController = cameraController;
        }

        public void SetDirection(Vector3 direction)
        {
            //カメラに一番近い前方ベクトルを取得
            Vector3 forward = verticalAdjuster.GetNearestDirection(direction);
            currentUpVector = verticalAdjuster.GetVerticalDirection(playerTransform.up);

            //カメラの前方ベクトルにバーチャルカメラの基準を設定
            cameraPivot.rotation = Quaternion.LookRotation(forward, currentUpVector);
        }

        private void Start()
        {
            verticalAdjuster = new VerticalAdjuster(virtualCamera.transform);
            cameraBrain = Camera.main.GetComponent<CinemachineBrain>();
        }

        private void Update()
        {
            if (!isEnabled.Value || playerTransform == null)
            {
                return;
            }

            EnablePlayerRotate();

            PerformAdditionalRotate();
            PerformPlayerRotate();
        }

        private void EnablePlayerRotate()
        {
            if (isPlayerRotating.Value || isInputRotating.Value)
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

        public void EnableAdditionalRotate(int value)
        {
            if (!isEnabled.Value || isPlayerRotating.Value || isInputRotating.Value)
            {
                return;
            }

            //プレイヤーの入力によって回転させる
            additionalRotation = Quaternion.AngleAxis(value * 90f, cameraPivot.up) * cameraPivot.rotation;
            isInputRotating.Value = true;
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

        private Quaternion additionalRotation;

        private void PerformAdditionalRotate()
        {
            if (!isInputRotating.Value)
            {
                return;
            }

            float rotationAngle = Quaternion.Angle(cameraPivot.rotation, additionalRotation);
            bool isLastRotation = PerformRotate(additionalRotation, rotationAngle);
            if (isLastRotation)
            {
                isInputRotating.Value = false;
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                Enable().Forget();
            }
        }

        private void OnTriggerExit(Collider other)
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
            
            SetDirection(cameraBrain.transform.forward);
            virtualCamera.Priority = 11;
            isEnabled.Value = true;
        }

        private void Disable()
        {
            cameraController.SetCameraRotation(cameraPivot.rotation);
            isEnabled.Value = false;
            virtualCamera.Priority = 0;
        }
    }
}