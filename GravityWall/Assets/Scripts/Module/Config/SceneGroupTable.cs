using System;
using System.Collections.Generic;
using CoreModule.Helper.Attribute;
using UnityEngine;

namespace Module.Config
{
    [Serializable]
    public class SceneGroup
    {
        [SerializeField] private SceneField mainScene;
        [SerializeField] private List<SceneField> scenes;
        
        public string GetMainScene()
        {
            return mainScene;
        }
        
        public IReadOnlyList<SceneField> GetScenes()
        {
            return scenes;
        }
    }

    [CreateAssetMenu(menuName = "SceneGroupTable", fileName = "SceneGroupTable")]
    public class SceneGroupTable : ScriptableObject
    {
        [SerializeField] private List<SceneGroup> sceneGroups = new List<SceneGroup>();

        public IReadOnlyList<SceneGroup> SceneGroups => sceneGroups;
    }
}