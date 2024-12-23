using System;
using UnityEngine;
using UnityEngine.Events;

namespace CoreModule.Helper
{
    public class OnTriggerEventBridge : MonoBehaviour
    {
        public UnityEvent<Collider> Enter;
        public UnityEvent<Collider> Stay;
        public UnityEvent<Collider> Exit;

        private void OnTriggerEnter(Collider collider)
        {
            Enter?.Invoke(collider);
        }

        private void OnTriggerStay(Collider collider)
        {
            Stay?.Invoke(collider);
        }

        private void OnTriggerExit(Collider collider)
        {
            Exit?.Invoke(collider);
        }
    }
}