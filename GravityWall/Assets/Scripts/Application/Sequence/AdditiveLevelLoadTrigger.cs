using System;
using System.Collections.Generic;
using Constants;
using CoreModule.Helper.Attribute;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace Application.Sequence
{
    public class AdditiveLevelLoadTrigger : MonoBehaviour
    {
        [SerializeField] private SceneField mainScene;
        [SerializeField] private List<SceneField> levelReference;
        [SerializeField] private bool triggerDetect;
        [SerializeField] private bool isTouched;
        
        public event Action OnSceneLoaded;
        public event Action<SceneField, List<SceneField>> OnLoadRequested;
        
        public void Load()
        {
            OnLoadRequested?.Invoke(mainScene, levelReference);
        }

        public void CallLoaded()
        {
            OnSceneLoaded?.Invoke();
        }

        public void Reset()
        {
            isTouched = false;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (triggerDetect && !isTouched && other.CompareTag(Tag.Player))
            {
                Load();
                isTouched = true;
            }
        }
        
    }
}