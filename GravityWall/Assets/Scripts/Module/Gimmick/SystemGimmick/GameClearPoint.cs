using System;
using Constants;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    public class GameClearPoint : MonoBehaviour
    {
        [SerializeField] private int stageId;
        public event Action OnClear;
        private bool firstTouch = false;
        
        public int StageId => stageId;

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