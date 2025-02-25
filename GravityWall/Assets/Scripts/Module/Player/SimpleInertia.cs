using System;
using UnityEngine;

namespace Module.Player
{
    /// <summary>
    /// 簡易的な慣性を表すクラス
    /// </summary>
    public class SimpleInertia : MonoBehaviour
    {
        private Rigidbody rigidbody;

        private Vector3 inertia;

        private void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        public void PerformInertia()
        {
            rigidbody.MovePosition(rigidbody.position + inertia);
        }

        public void OnCollisionEnter()
        {
            SetInertia(Vector3.zero);
        }

        public void AddInertia(Vector3 inertia)
        {
            if (!enabled)
            {
                return;
            }

            this.inertia += inertia;
        }

        public void SetInertia(Vector3 inertia)
        {
            if (!enabled)
            {
                return;
            }

            this.inertia = inertia;
        }
    }
}