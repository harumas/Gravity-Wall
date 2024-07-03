using System;
using Core.Input;
using UnityEngine;

namespace Module.Character
{
    public class PlayerTargetSyncer : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float rotateSpeed;

        private Vector2 input;
        private InputEvent controlEvent;

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
            PerformBodyRotate();
        }

        private void PerformBodyRotate()
        {
            if (input != Vector2.zero)
            {
                Vector3 inputDirection = transform.rotation * new Vector3(input.x, 0f, input.y);
                UGizmo.UGizmos.DrawArrow(transform.position, transform.position + inputDirection, Color.blue);
                Quaternion targetRotation = Quaternion.LookRotation(inputDirection, transform.up) * transform.rotation;

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed);
            }
        }
    }
}