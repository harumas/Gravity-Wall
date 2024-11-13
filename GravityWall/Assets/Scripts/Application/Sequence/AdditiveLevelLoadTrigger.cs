using System;
using System.Collections.Generic;
using Constants;
using CoreModule.Helper.Attribute;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Application.Sequence
{
    public class AdditiveLevelLoadTrigger : MonoBehaviour
    {
        [SerializeField] private SceneField mainScene;
        [SerializeField] private List<SceneField> levelReference;
        [SerializeField] private bool triggerDetect;
        [SerializeField] private bool isLoaded;

        public event Action<SceneField, List<SceneField>> OnLoadRequested;
        
        public void Load()
        {
            OnLoadRequested?.Invoke(mainScene, levelReference);
        }

        public void Reset()
        {
            isLoaded = false;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (triggerDetect && !isLoaded && other.CompareTag(Tag.Player))
            {
                Load();
                isLoaded = true;
            }
        }
        
    }
}