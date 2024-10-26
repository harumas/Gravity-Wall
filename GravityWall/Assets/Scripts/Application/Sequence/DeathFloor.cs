using System;
using Constants;
using UnityEngine;

namespace Application.Sequence
{
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