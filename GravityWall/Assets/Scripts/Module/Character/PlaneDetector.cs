using Module.Gimmick;
using UGizmo;
using UnityEngine;

namespace Module.Character
{
    public class PlaneDetector : MonoBehaviour
    {
        [SerializeField] private float detectHoldAngle = 1f;
        [SerializeField] private float detectRayRadius = 0.6f;
        [SerializeField] private float detectRayRange = 0.6f;
        [SerializeField] private float downRayDistance = 0.6f;
        [SerializeField] private PlayerTargetSyncer targetSyncer;

        private Collider currentCollider;
        private Vector3 prevDir;
        private bool isLegalStep;

        private void Update()
        {
            isLegalStep = IsLegalStep();
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

            //壁の判定
            Ray ray = new Ray(transform.position, targetDir);
            bool isHit = Physics.SphereCast(ray, detectRayRadius, out RaycastHit hitInfo, detectRayRange);

            UGizmos.DrawSphereCast(ray.origin, detectRayRadius, ray.direction, detectRayRange, isHit, hitInfo);

            //進行方向に壁がある場合は重力変更可能
            if (isHit)
            {
                return true;
            }

            //床の判定
            Vector3 origin = transform.position + targetDir * detectRayRange;
            Vector3 direction = -transform.up;

            isHit = Physics.Raycast(origin, direction, out hitInfo, downRayDistance);

            UGizmos.DrawRay(origin, direction * downRayDistance, Color.blue);

            //床と自分のy軸が等しくない場合は、重力変更する必要がある
            if (isHit)
            {
                return transform.up != hitInfo.normal;
            }

            //何もヒットしていない場合は、進行方向に何も存在しないため、重力変更する必要がある
            return true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            bool isSafeAngle = Vector3.Angle(-transform.up, collision.GetContact(0).normal) > detectHoldAngle;

            LocalGravity localGravity = collision.collider.GetComponent<LocalGravity>();
            bool isActive = localGravity == null || (localGravity != null && localGravity.enabled);

            if (isSafeAngle && isActive)
            {
                currentCollider = collision.collider;
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            ContactPoint contact = collision.GetContact(0);
            float minDistance = Vector3.SqrMagnitude(contact.point - (transform.position + prevDir));

            for (int i = 0; i < collision.contactCount - 1; i++)
            {
                ContactPoint current = collision.GetContact(i);
                float distance = Vector3.SqrMagnitude(current.point - (transform.position + prevDir));
                if (distance < minDistance)
                {
                    minDistance = distance;
                    contact = current;
                }
            }

            if (isLegalStep && currentCollider == collision.collider)
            {
                Gravity.SetValue(-contact.normal);
            }
        }
    }
}