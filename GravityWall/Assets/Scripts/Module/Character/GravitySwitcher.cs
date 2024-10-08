﻿using Constants;
using Module.Gravity;
using UGizmo;
using UnityEngine;

namespace Module.Character
{
    public class GravitySwitcher : MonoBehaviour
    {
        [SerializeField] private bool isEnabled = true;
        [SerializeField] private float detectHoldAngle = 1f;
        [SerializeField] private float detectRayRadius = 0.6f;
        [SerializeField] private float detectRayRange = 0.6f;
        [SerializeField] private float downRayDistance = 0.6f;
        [SerializeField] private PlayerTargetSyncer targetSyncer;

        private Vector3 prevDir;
        private ContactPoint nearestContact;
        private bool doSwitchGravity;
        private bool hasHeadObject;
        private bool isLegalStep;

        private void Update()
        {
            isLegalStep = isEnabled && IsLegalStep();
        }

        private void FixedUpdate()
        {
            if (doSwitchGravity && !hasHeadObject)
            {
                WorldGravity.Instance.SetValue(-nearestContact.normal);
            }

            doSwitchGravity = false;
            hasHeadObject = false;
        }

        private void Enable()
        {
            isEnabled = true;
        }

        private void Disable()
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

            UGizmos.DrawSphereCast(ray.origin, detectRayRadius, ray.direction, detectRayRange, isHit, hitInfo);

            return isHit;
        }

        private bool IsFloor(Vector3 targetDir, out Vector3 normal)
        {
            Vector3 origin = transform.position + targetDir * detectRayRange;
            Vector3 direction = -transform.up;

            bool isHit = Physics.Raycast(origin, direction, out RaycastHit hitInfo, downRayDistance);

            UGizmos.DrawRay(origin, direction * downRayDistance, Color.blue);

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
                }

                //重力変更
                if (isLegalStep)
                {
                    doSwitchGravity = true;
                }
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