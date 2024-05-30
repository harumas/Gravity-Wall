using System;
using Module.Gimmick;
using UnityEngine;

namespace Module.Character
{
    [Serializable]
    public class PlaneDetector : MonoBehaviour
    {
        [SerializeField] private float detectHoldAngle = 1f;
        private Collider currentCollider;

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
            if (currentCollider == collision.collider)
            {
                Gravity.SetValue(-collision.GetContact(0).normal);
            }
        }
    }
}