using System;
using Constants;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Application.Sequence
{
    public class GameClearPoint : MonoBehaviour
    {
        public event Action OnClear;
        private bool firstTouch = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!firstTouch && other.CompareTag(Tag.Player))
            {
                Clear();
            }
        }

        private void Clear()
        {
            OnClear?.Invoke();
        }
    }
}