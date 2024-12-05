using System;
using UnityEngine;

namespace CoreModule.Helper
{
    /// <summary>
    /// コリジョンイベントを仲介するクラス
    /// </summary>
    public class OnCollisionEventBridge : MonoBehaviour
    {
        public event Action<Collision> Enter;
        public event Action<Collision> Stay;
        public event Action<Collision> Exit;

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