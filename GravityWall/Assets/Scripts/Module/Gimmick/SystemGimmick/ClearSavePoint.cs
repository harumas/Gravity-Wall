using System;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    public class ClearSavePoint : MonoBehaviour
    {
        [SerializeField] private int stageId;

        public event Action<int> OnSave;

        public void Save()
        {
            OnSave?.Invoke(stageId);
        }
    }
}
