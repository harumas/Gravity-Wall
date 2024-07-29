using System;
using Core.Input;
using UnityEngine;
using UnityEngine.Serialization;

namespace Module.Character
{
    public class PlayerTargetSyncer : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Transform axis;
        [SerializeField] private float damping;

        private Vector2 input;
        private InputEvent controlEvent;

        public Vector3 GetTargetDirection()
        {
            if (input == Vector2.zero)
            {
                return Vector3.zero;
            }

            Vector3 inputDirection = target.rotation * new Vector3(input.x, 0f, input.y);
            Vector3 planedDirection = Vector3.ProjectOnPlane(inputDirection, axis.up);

            return planedDirection;
        }

        private void Start()
        {
            controlEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.Player.Move);
        }

        private void Update()
        {
            input = controlEvent.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            if (input == Vector2.zero)
            {
                return;
            }

            PerformBodyRotate();
        }

        private void PerformBodyRotate()
        {
            Vector3 targetDirection = GetTargetDirection();
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, axis.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, damping);
        }

        private void OnDrawGizmos()
        {
            Vector3 targetDirection = GetTargetDirection();
            UGizmo.UGizmos.DrawArrow(transform.position, transform.position + targetDirection.normalized, Color.blue, headLength: 0.4f, width: 0.2f);
        }
    }
}