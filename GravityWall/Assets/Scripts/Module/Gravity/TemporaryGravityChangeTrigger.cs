using System;
using Constants;
using UnityEngine;
using UnityEngine.Serialization;

namespace Module.Gravity
{
    public class TemporaryGravityChangeTrigger : MonoBehaviour
    {
        [SerializeField] private float temporaryGravityMultiplier;
        [SerializeField] private Vector3 constraintDirection;
        [SerializeField] private float velocityMultiplier;

        private bool isPlayerEnter;
        private LocalGravity targetGravity;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(Tag.Player) && IsConstrainedDirection(other.transform.up))
            {
                targetGravity = other.gameObject.GetComponent<LocalGravity>();
                Rigidbody rig = other.gameObject.GetComponent<Rigidbody>();
                rig.velocity *= velocityMultiplier;
                isPlayerEnter = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag(Tag.Player))
            {
                isPlayerEnter = false;
            }
        }
        
        private bool IsConstrainedDirection(Vector3 direction)
        {
            return Vector3.Dot(constraintDirection, direction) > 0.5f;
        }

        private void FixedUpdate()
        {
            if (isPlayerEnter)
            {
                targetGravity.SetMultiplierAtFrame(temporaryGravityMultiplier);
            }
        }
    }
}