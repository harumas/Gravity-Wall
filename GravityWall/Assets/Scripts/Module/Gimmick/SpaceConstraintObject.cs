using System;
using GravityWall;
using UnityEngine;

namespace Module.Gimmick
{
    public class SpaceConstraintObject : MonoBehaviour
    {
        private Rigidbody rigBody;

        private void Start()
        {
            rigBody = transform.parent.GetComponent<Rigidbody>();
        }

        public void Enable()
        {
            enabled = true;
        }

        public void Disable()
        {
            enabled = false;
        }

        private void OnTriggerStay(Collider other)
        {
            if (enabled && other.gameObject.CompareTag(Tag.Player))
            {
                rigBody.constraints = RigidbodyConstraints.FreezeAll;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (enabled && other.gameObject.CompareTag(Tag.Player))
            {
                rigBody.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
    }
}