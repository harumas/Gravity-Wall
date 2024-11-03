using System;
using UnityEngine;

namespace Module.Character
{
    /// <summary>
    /// 簡易的な慣性を表すクラス
    /// </summary>
    public class SimpleInertia
    {
        private readonly Rigidbody rigidbody;

        private Vector3 inertia;

        public SimpleInertia(Rigidbody rigidbody)
        {
            this.rigidbody = rigidbody;
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
            this.inertia += inertia;
        }

        public void SetInertia(Vector3 inertia)
        {
            this.inertia = inertia;
        }
    }
}