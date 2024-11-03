using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Application.Sequence
{
    public class AdditiveLevelLoadTrigger : MonoBehaviour
    {
        [SerializeField] private AssetReference levelReference;

        public event Action<AssetReference> OnLoadRequested;
        public event Action<AssetReference> OnUnloadRequested;
        
        public void Load()
        {
            OnLoadRequested?.Invoke(levelReference);
        }
        
        public void Unload()
        {
            OnUnloadRequested?.Invoke(levelReference);
        }
    }
}