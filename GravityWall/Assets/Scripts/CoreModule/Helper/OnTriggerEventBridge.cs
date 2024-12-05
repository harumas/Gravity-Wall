using System;
using UnityEngine;

namespace CoreModule.Helper
{
    /// <summary>
    /// トリガーイベントを仲介するクラス
    /// </summary>
    public class OnTriggerEventBridge : MonoBehaviour
    {
        public event Action<Collider> Enter;
        public event Action<Collider> Stay;
        public event Action<Collider> Exit;

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