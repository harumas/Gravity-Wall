using System;
using Constants;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Application.Sequence
{
    public class GameClearPoint : MonoBehaviour
    {
        public event Action OnClear;
        private bool firstTouch = true;

        private void OnTriggerEnter(Collider other)
        {
            if (!firstTouch && other.CompareTag(Tag.Player))
            {
                Clear();
            }
        }

        public void Clear()
        {
            OnClear?.Invoke();
            firstTouch = false;
        }
    }
}