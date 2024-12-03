using System;
using Constants;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    public class GameClearPoint : MonoBehaviour
    {
        public event Action OnClear;
        private bool isFirstTouch = true;

        private void OnTriggerEnter(Collider other)
        {
            if (isFirstTouch && other.CompareTag(Tag.Player))
            {
                Clear();
                isFirstTouch = false;
            }
        }

        private void Clear()
        {
            OnClear?.Invoke();
        }
    }
}