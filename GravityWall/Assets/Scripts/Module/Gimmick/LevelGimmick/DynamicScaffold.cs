using System.Threading;
using Constants;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Module.Player;
using R3;
using UnityEngine;

namespace Module.Gimmick.LevelGimmick
{
    [RequireComponent(typeof(Rigidbody))]
    public class DynamicScaffold : GimmickObject
    {
        [SerializeField] private Transform pointA;
        [SerializeField] private Transform pointB;
        [Header("移動速度")] [SerializeField] private float moveSpeed;

        [Header("目標地点で待機する時間")] [SerializeField] private float stopDuration;

        [Header("引っかかりを待つ時間")] [SerializeField] private float reverseDuration;

        [Header("最初から動かすか")] [SerializeField] private bool enableOnAwake = true;

        [SerializeField] private GimmickObject[] observedSwitches;
        [SerializeField] private int switchMaxCount = 1;
        [SerializeField] private int contactCount;

        private int switchCount = 0;
        private CancellationTokenSource cTokenSource;
        private Rigidbody rigBody;
        private Vector3 previousTargetPosition;
        private Vector3 previousPosition;
        private Vector3 moveDelta;
        private Transform currentTarget;
        private IPushable pushement;
        private float trappedTimer;

        private const float StopThreshold = 0.01f;

        private CancellationTokenSource cancellationToken;

        private void Awake()
        {
            rigBody = GetComponent<Rigidbody>();

            SetUpRigidbody();
        }

        private void SetUpRigidbody()
        {
            //Rigidbodyのセットアップ
            rigBody.mass = Mathf.Infinity;
            rigBody.drag = Mathf.Infinity;
            rigBody.useGravity = false;
            rigBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rigBody.constraints = RigidbodyConstraints.FreezeRotation;

            Reset();
        }

        private void Start()
        {
            foreach (GimmickObject gimmick in observedSwitches)
            {
                gimmick.IsEnabled.Skip(1).Subscribe(UpdateMoveState).AddTo(this);
            }

            if (enableOnAwake)
            {
                Enable();
            }
        }

        private void UpdateMoveState(bool switchEnabled)
        {
            switchCount += switchEnabled ? 1 : -1;
            bool isMove = switchCount >= switchMaxCount;

            if (isMove)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }


        public override void Enable(bool doEffect = true)
        {
            if (isEnabled.Value)
            {
                return;
            }

            cTokenSource = new CancellationTokenSource();
            MoveLoop().Forget();
            isEnabled.Value = true;
        }

        public override void Disable(bool doEffect = true)
        {
            if (!isEnabled.Value)
            {
                return;
            }

            cTokenSource.Cancel();
            isEnabled.Value = false;
        }

        public override void Reset()
        {
            cTokenSource?.Cancel();
            rigBody.position = pointA.position;
            previousPosition = rigBody.position;
            currentTarget = pointB;
        }

        private async UniTaskVoid MoveLoop()
        {
            CancellationToken cancelOnDestroyToken = this.GetCancellationTokenOnDestroy();
            CancellationTokenSource canceller = CancellationTokenSource.CreateLinkedTokenSource(cancelOnDestroyToken, cTokenSource.Token);
            CancellationToken cancellationToken = canceller.Token;
            while (!cancellationToken.IsCancellationRequested)
            {
                bool arrived = false;

                await foreach (var _ in UniTaskAsyncEnumerable.EveryUpdate(PlayerLoopTiming.FixedUpdate).WithCancellation(cancellationToken))
                {
                    Vector3 position = GetMoveTarget();
                    arrived = position == currentTarget.position;

                    //ターゲットに到達 or 指定時間引っかかったら終了
                    //対象を切り替える
                    if (arrived || CheckTrapped(position))
                    {
                        SwitchTarget();
                        break;
                    }

                    rigBody.MovePosition(position);
                    MovePlayer();
                }

                //目標地点に到達できたら指定時間待機
                if (arrived)
                {
                    await UniTask.WaitForSeconds(stopDuration, delayTiming: PlayerLoopTiming.FixedUpdate, cancellationToken: cancellationToken);
                }
            }
        }

        private Vector3 GetMoveTarget()
        {
            float distance = Vector3.Distance(transform.position, currentTarget.position);
            Vector3 position = Vector3.Lerp(transform.position, currentTarget.position, moveSpeed / distance);

            //ターゲットに十分近い場合はターゲット座標までの差分を移動量とする
            if (distance < StopThreshold)
            {
                position = currentTarget.position;
            }

            return position;
        }

        private bool CheckTrapped(Vector3 targetPosition)
        {
            if (targetPosition == previousPosition)
            {
                //床がずっと同じ場所にとどまっている場合はカウントする
                trappedTimer += Time.fixedDeltaTime;

                //待機時間を超えたら引っかかっているとする
                if (trappedTimer >= reverseDuration)
                {
                    trappedTimer = 0f;
                    return true;
                }
            }
            else
            {
                trappedTimer = 0f;
            }

            return false;
        }

        private void SwitchTarget()
        {
            previousPosition = rigBody.position;
            moveDelta = Vector3.zero;
            currentTarget = currentTarget == pointA ? pointB : pointA;
        }

        private void MovePlayer()
        {
            if (pushement != null)
            {
                //床の移動量分、プレイヤーを動かす
                moveDelta = rigBody.position - previousPosition;
                pushement.AddExternalPosition(moveDelta);
            }

            previousPosition = rigBody.position;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.TryGetComponent(out IPushable pushable))
            {
                contactCount++;

                //床に設置したプレイヤーを取得
                if (contactCount == 1 && other.gameObject.CompareTag(Tag.Player))
                {
                    pushement = pushable;
                }
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.TryGetComponent(out IPushable _))
            {
                contactCount--;

                if (contactCount == 0 && other.gameObject.CompareTag(Tag.Player))
                {
                    //コリジョンから離れる時は、慣性を付与する
                    pushement.AddInertia(moveDelta);
                    pushement = null;
                }
            }
        }

        void OnDestroy()
        {
            if (cancellationToken == null) return;
            cancellationToken.Cancel();
            cancellationToken.Dispose();
        }
    }
}