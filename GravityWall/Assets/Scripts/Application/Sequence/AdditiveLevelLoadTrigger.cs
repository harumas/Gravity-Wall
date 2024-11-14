using System;
using System.Collections.Generic;
using CoreModule.Helper.Attribute;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Application.Sequence
{
    public class AdditiveLevelLoadTrigger : MonoBehaviour
    {
        [SerializeField] private SceneField mainScene;
        [SerializeField] private List<SceneField> levelReference;

        public event Action<SceneField, List<SceneField>> OnLoadRequested;
        public event Action<SceneField, List<SceneField>> OnUnloadRequested;

        public void Load()
        {
            OnLoadRequested?.Invoke(mainScene, levelReference);
        }

        public void Unload()
        {
            OnUnloadRequested?.Invoke(mainScene, levelReference);
        }
    }
}