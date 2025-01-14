using System;
using UnityEngine;
using UnityEngine.Events;

namespace CoreModule.Helper
{
    public class OnCollisionEventBridge : MonoBehaviour
    {
        public UnityEvent<Collision> Enter;
        public UnityEvent<Collision> Stay;
        public UnityEvent<Collision> Exit;

        private void OnCollisionEnter(Collision collision)
        {
            Enter?.Invoke(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            Stay?.Invoke(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            Exit?.Invoke(collision);
        }
    }
}