using System;
using UnityEngine;

namespace Application.Sequence
{
    public class AdditiveLevelLoadTrigger : MonoBehaviour
    {
        [SerializeField] private string levelName;

        public event Action<string> OnLoadRequested;
        public event Action<string> OnUnloadRequested;
        
        public void Load()
        {
            OnLoadRequested?.Invoke(levelName);
        }
        
        public void Unload()
        {
            OnUnloadRequested?.Invoke(levelName);
        }
    }
}