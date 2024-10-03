using System;
using Cinemachine;
using CoreModule.Helper;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Module.Gimmick.LevelMask;
using R3;
using UnityEngine;

namespace Module.Gimmick
{
    /// <summary>
    /// レベルボリュームを俯瞰するカメラ
    /// </summary>
    public class LevelVolumeCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Ease easeType;

        [Header("回転のイージング係数")]
        [SerializeField]
        private float rotateStep;

        [SerializeField] private SerializableReactiveProperty<bool> isEnabled;
        [SerializeField] private SerializableReactiveProperty<bool> isPlayerRotating;
        [SerializeField] private SerializableReactiveProperty<bool> isInputRotating;

        public ReadOnlyReactiveProperty<bool> IsEnabled => isEnabled;
        public Observable<bool> IsRotating => Observable.Merge(isPlayerRotating, isInputRotating);
        public Observable<Quaternion> Rotation => Observable.EveryValueChanged(cameraPivot, target => target.rotation);

        private MaskVolume maskVolume;
        private CinemachineBrain cameraBrain;
        private VerticalAdjuster verticalAdjuster;
        private Transform playerTransform;
        private Vector3 currentUpVector;

        public void AssignPlayerTransform(Transform playerTransform)
        {
            this.playerTransform = playerTransform;
        }

        private void Awake()
        {
            verticalAdjuster = new VerticalAdjuster();
            maskVolume = transform.parent.GetComponent<MaskVolume>();
            cameraBrain = Camera.main.GetComponent<CinemachineBrain>();

            if (maskVolume == null)
            {
                Debug.LogError("MaskVolumeが見つかりませんでした");
                return;
            }

            AssignVolumeEvent();

            enableCount = 0;
        }

        private void Update()
        {
            if (!isEnabled.Value || playerTransform == null)
            {
                return;
            }
            
            Debug.Log(cameraPivot.forward);

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
            
            Debug.Log(cameraPivot.forward);

            //90度になったらカメラの向きを変える
            if (direction != Vector3.zero && direction != currentUpVector)
            {
                Debug.Log(direction);
                Debug.Log(currentUpVector);
                currentUpVector = direction;
                isPlayerRotating.Value = true;
                Debug.Log("true!");
            }
        }

        public void EnableAdditionalRotate(int value)
        {
            if (isPlayerRotating.Value || isInputRotating.Value)
            {
                return;
            }

            //プレイヤーの入力によって回転させる
            additionalRotation = Quaternion.AngleAxis(value * 90f, cameraPivot.up) * cameraPivot.rotation;
            isInputRotating.Value = true;
                Debug.Log("true!");
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

        //TODO: 後で消す
        private static int enableCount = 0;

        private async void Enable()
        {
            InitializeDirection();
            virtualCamera.Priority = 11;

            if (enableCount > 0)
            {
                //ブレンドしてる間は有効化しない
                TimeSpan blendSpan = TimeSpan.FromSeconds(cameraBrain.m_DefaultBlend.BlendTime + 0.1f);
                await UniTask.Delay(blendSpan, cancellationToken: destroyCancellationToken);
            }

            enableCount++;
            isEnabled.Value = true;
        }

        private void InitializeDirection()
        {
            //カメラに一番近い前方ベクトルを取得
            Vector3 forward = verticalAdjuster.GetNearestDirection(cameraBrain.transform.forward);
            currentUpVector = verticalAdjuster.GetVerticalDirection(playerTransform.up);

            Debug.Log(forward);

            //カメラの前方ベクトルにバーチャルカメラの基準を設定
            cameraPivot.rotation = Quaternion.LookRotation(forward, currentUpVector);
            
            
            Debug.Log(cameraPivot.forward);
        }

        private void Disable()
        {
            isEnabled.Value = false;
            virtualCamera.Priority = 0;
        }
    }
}