using Constants;
using CoreModule.Helper;
using Module.Gravity;
using R3;
using UGizmo;
using UnityEngine;

namespace Module.Player
{
    public class GravitySwitcher : MonoBehaviour
    {
        [SerializeField] private bool isEnabled = true;
        [SerializeField] private bool canSwitchGravity = true;

        [Header("重力変化の制限がかかる角度")][SerializeField] private float constrainedAngleThreshold;

        [Header("入力が開始されてから重力変更が可能になるまでの秒数")]
        [SerializeField]
        private float allowAngleChangeDuration;

        [Header("重力変化までの秒数")][SerializeField] private float angleChangeDuration;
        [SerializeField] private float detectHoldAngle = 1f;
        [SerializeField] private float detectRayRadius = 0.6f;
        [SerializeField] private float detectRayRange = 0.6f;
        [SerializeField] private float downRayDistance = 0.6f;
        [SerializeField] private PlayerTargetSyncer targetSyncer;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private bool isGrounding;

        private Vector3 prevDir;
        private (Vector3 xv, Vector3 yv) lastMovement;
        private Vector3 nearestNormal;
        private ContactPoint nearestContact;
        private bool doSwitchGravity;
        private bool hasHeadObject;
        private bool isLegalStep;
        private bool isInputMoving;

        private ThresholdChecker rotateAngleChecker;
        private DelayableProperty<bool> canRotateProperty = new();

        private void Awake()
        {
            rotateAngleChecker = new ThresholdChecker(constrainedAngleThreshold, angleChangeDuration);
            rotateAngleChecker.Enable();

            //プレイヤーの回転が終わったらチェッカーを有効化する
            playerController.IsRotating.Subscribe(value =>
                {
                    if (!value)
                    {
                        rotateAngleChecker.Enable();
                    }
                })
                .AddTo(this);

            //ジャンプ中はチェッカーを無効化する
            playerController.IsJumping.Subscribe(value =>
                {
                    if (value)
                    {
                        rotateAngleChecker.Disable();
                    }

                    isGrounding = !value;
                })
                .AddTo(this);

            playerController.OnMove.Subscribe(value => { lastMovement = value; }).AddTo(this);
        }

        public void OnMoveInput(Vector2 input)
        {
            bool isMoving = input != Vector2.zero;
            float delay = isMoving ? allowAngleChangeDuration : 0f;

            canRotateProperty.Assign(isMoving, delay);
        }

        private void Update()
        {
            isLegalStep = isEnabled && canSwitchGravity && IsLegalStep();

            canRotateProperty.Update();
        }

        private void FixedUpdate()
        {
            if (isEnabled)
            {
                SwitchGravity();
            }
        }

        private void SwitchGravity()
        {
            //角度の差を求める
            float angle = Vector3.Angle(transform.up, nearestNormal);
            angle = Mathf.Max(angle, Mathf.Epsilon);

            //角度が一定以下の場合は重力変更を行わない
            if ((playerController.IsRotating.CurrentValue ||
                 rotateAngleChecker.IsOverThreshold(angle) && !canRotateProperty.Value ||
                 rotateAngleChecker.TryThresholdCount(angle, isGrounding)) &&
                isGrounding)
            {
                canSwitchGravity = false;
                return;
            }

            canSwitchGravity = true;
            rotateAngleChecker.Disable();

            if (doSwitchGravity && !hasHeadObject)
            {
                WorldGravity.Instance.SetValue(-nearestNormal);
            }

            doSwitchGravity = false;
            hasHeadObject = false;
        }

        public void Enable()
        {
            Debug.Log("Enable!!");
            isEnabled = true;
        }

        public void Disable()
        {
            isEnabled = false;
        }

        /// <summary>
        /// 重力変更が可能のな床かどうかを判定します
        /// </summary>
        /// <returns></returns>
        private bool IsLegalStep()
        {
            Vector3 targetDir = targetSyncer.GetTargetDirection();

            if (targetDir == Vector3.zero)
            {
                targetDir = prevDir;
            }

            prevDir = targetDir;

            //進行方向に壁がある場合は重力変更可能
            if (IsWall(targetDir))
            {
                return true;
            }

            //床と自分のy軸が等しくない場合は、重力変更する必要がある
            if (IsFloor(targetDir, out Vector3 normal))
            {
                return transform.up != normal;
            }


            //何もヒットしていない場合は、進行方向に何も存在しないため、重力変更する必要がある
            return true;
        }

        private bool IsWall(Vector3 targetDir)
        {
            Ray ray = new Ray(transform.position, targetDir);
            bool isHit = Physics.SphereCast(ray, detectRayRadius, out RaycastHit hitInfo, detectRayRange);

#if UNITY_EDITOR
            UGizmos.DrawSphereCast(ray.origin, detectRayRadius, ray.direction, detectRayRange, isHit, hitInfo);
#endif

            return isHit;
        }

        private bool IsFloor(Vector3 targetDir, out Vector3 normal)
        {
            Vector3 origin = transform.position + targetDir * detectRayRange;
            Vector3 direction = -transform.up;

            bool isHit = Physics.Raycast(origin, direction, out RaycastHit hitInfo, downRayDistance);

#if UNITY_EDITOR
            UGizmos.DrawRay(origin, direction * downRayDistance, Color.blue);
#endif

            normal = hitInfo.normal;

            return isHit;
        }

        private void OnCollisionStay(Collision collision)
        {
            ContactPoint contact = GetNearestContact(collision);
            (bool canTouch, _) = CheckCollision(contact);

            if (HasHeadObject(collision))
            {
                hasHeadObject = true;
            }

            if (canTouch)
            {
                if (!hasHeadObject)
                {
                    nearestContact = GetNearestContact(collision);

                    if (DetectWall(out Vector3 correctNormal))
                    {
                        nearestNormal = correctNormal;
                    }
                    else
                    {
                        nearestNormal = nearestContact.normal;
                    }
                }

                //重力変更
                if (isLegalStep)
                {
                    doSwitchGravity = true;
                }
            }
        }

        private bool DetectWall(out Vector3 correctNormal)
        {
            float detectRange = 0.2f;
            float detectDistance = 0.55f;
            Vector3 moveDirection = lastMovement.xv.normalized;
            Vector3 right = Vector3.Cross(transform.up, moveDirection);
            Vector3 origin = transform.position;

            int layerMask = Layer.Mask.Base | Layer.Mask.IgnoreGravity | Layer.Mask.Gravity;
            Vector3 rOrigin = origin + right * detectRange;
            bool isHitR = Physics.Raycast(rOrigin, moveDirection, out RaycastHit hitR, detectDistance, layerMask);

            Vector3 lOrigin = origin + -right * detectRange;
            bool isHitL = Physics.Raycast(lOrigin, moveDirection, out RaycastHit hitL, detectDistance, layerMask);

#if UNITY_EDITOR
            UGizmos.DrawLine(rOrigin, rOrigin + moveDirection * detectDistance, Color.green);
            UGizmos.DrawLine(lOrigin, lOrigin + moveDirection * detectDistance, Color.green);
#endif

            if (isHitL && isHitR && hitR.normal == hitL.normal)
            {
                correctNormal = hitR.normal;
                return true;
            }
            else
            {
                correctNormal = Vector3.zero;
                return false;
            }
        }

        private bool HasHeadObject(Collision collision)
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                ContactPoint contact = collision.GetContact(i);
                (_, bool isHeadObject) = CheckCollision(contact);

                if (isHeadObject)
                {
                    return true;
                }
            }

            return false;
        }

        private (bool canTouch, bool isHeadObject) CheckCollision(in ContactPoint contact)
        {
            //真上にオブジェクトが衝突した場合は、角度によってくっつくかを判定する
            bool isSafeAngle = Vector3.Angle(-transform.up, contact.normal) > detectHoldAngle;

            //衝突したオブジェクトが重力の影響を受けるか判定
            bool isGravityCollider = contact.otherCollider.gameObject.layer != Layer.IgnoreGravity;
            LocalGravity localGravity = contact.otherCollider.GetComponent<LocalGravity>();

            bool isObjectHit = localGravity != null && localGravity.enabled;
            bool canTouch = isGravityCollider && (localGravity == null || isObjectHit && isSafeAngle);

            return (canTouch, isObjectHit && !isSafeAngle);
        }

        private ContactPoint GetNearestContact(Collision collision)
        {
            ContactPoint contact = nearestContact;
            float minDistance = float.MaxValue;

            //最も自分の体に近い壁を判定
            //CapsuleColliderのため、側面の壁が優先されるはず
            for (int i = 0; i < collision.contactCount; i++)
            {
                ContactPoint current = collision.GetContact(i);
                float distance = Vector3.SqrMagnitude(current.point - (transform.position + prevDir));
                if (distance < minDistance)
                {
                    minDistance = distance;
                    contact = current;
                }
            }

            return contact;
        }
    }
}