using System;
using Constants;
using UnityEngine;

namespace Application.Sequence
{
    /// <summary>
    /// 死亡通知を送信するコンポーネント
    /// </summary>
    public class DeathFloor : MonoBehaviour
    {
        public event Action OnEnter;
        
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(Tag.Player))
            {
                OnEnter?.Invoke();
            }
        }
    }
}