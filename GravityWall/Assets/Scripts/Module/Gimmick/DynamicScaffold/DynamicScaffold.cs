using System;
using System.Threading;
using Constants;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Module.Character;
using UnityEngine;

namespace Module.Gimmick.DynamicScaffold
{
    [RequireComponent(typeof(Rigidbody))]
    public class DynamicScaffold : MonoBehaviour
    {
        [SerializeField] private Transform pointA;
        [SerializeField] private Transform pointB;
        [Header("移動速度")] [SerializeField] private float moveSpeed;

        [Header("目標地点で待機する時間")]
        [SerializeField]
        private float stopDuration;

        [Header("引っかかりを待つ時間")]
        [SerializeField]
        private float reverseDuration;

        private Rigidbody rigBody;
        private Vector3 previousTargetPosition;
        private Vector3 previousPosition;
        private Vector3 moveDelta;
        private Transform currentTarget;
        private PlayerController playerController;
        private float trappedTimer;

        private const float StopThreshold = 0.01f;

        private void Awake()
        {
            rigBody = GetComponent<Rigidbody>();

            //Rigidbodyのセットアップ
            rigBody.mass = Mathf.Infinity;
            rigBody.drag = Mathf.Infinity;
            rigBody.useGravity = false;
            rigBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rigBody.constraints = RigidbodyConstraints.FreezeRotation;

            rigBody.position = pointA.position;
            previousPosition = rigBody.position;
            currentTarget = pointB;
        }

        private void Start()
        {
            MoveLoop().Forget();
        }

        private async UniTaskVoid MoveLoop()
        {
            CancellationToken cancellationToken = this.GetCancellationTokenOnDestroy();

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
            if (playerController != null)
            {
                //床の移動量分、プレイヤーを動かす
                moveDelta = rigBody.position - previousPosition;
                playerController.AddExternalPosition(moveDelta);
            }

            previousPosition = rigBody.position;
        }

        private void OnCollisionEnter(Collision other)
        {
            //床に設置したプレイヤーを取得
            if (other.gameObject.CompareTag(Tag.Player))
            {
                playerController = other.gameObject.GetComponent<PlayerController>();
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag(Tag.Player))
            {
                //コリジョンから離れる時は、慣性を付与する
                playerController.AddInertia(moveDelta);
                playerController = null;
            }
        }
    }
}