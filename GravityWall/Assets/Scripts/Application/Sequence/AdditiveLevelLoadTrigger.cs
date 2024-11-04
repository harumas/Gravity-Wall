using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Application.Sequence
{
    public class AdditiveLevelLoadTrigger : MonoBehaviour
    {
        [SerializeField] private List<AssetReference>  levelReference;

        public event Action<List<AssetReference>> OnLoadRequested;
        public event Action<List<AssetReference>> OnUnloadRequested;
        
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