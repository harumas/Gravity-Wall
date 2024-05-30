using System;
using Core.Input;
using UnityEngine;

namespace Module.Player
{
    public class PlayerRotator : MonoBehaviour
    {
        [SerializeField] private float rotateSpeed;
        [SerializeField] private float damping;

        private InputEvent rotateEvent;

        private void Start()
        {
            rotateEvent = InputActionProvider.Instance.CreateEvent(ActionGuid.Player.Look);
        }

        private void FixedUpdate()
        {
            Quaternion target = Quaternion.AngleAxis(rotateEvent.ReadValue<float>() * rotateSpeed, transform.up) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, target, damping);
        }
    }
}