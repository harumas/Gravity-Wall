using System;
using Module.Gimmick;
using UGizmo;
using UnityEngine;

namespace Module.Player
{
    [Serializable]
    public class PlaneDetector : MonoBehaviour
    {
        private Collider currentCollider;

        private void OnCollisionEnter(Collision collision)
        {
            currentCollider = collision.collider;
        }

        private void OnCollisionStay(Collision collision)
        {
            if (currentCollider == collision.collider)
            {
                Gravity.SetDir(collision.GetContact(0).normal);
            }
        }
    }
}