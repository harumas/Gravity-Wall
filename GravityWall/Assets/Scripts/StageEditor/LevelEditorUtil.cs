using System.Collections.Generic;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace StageEditor
{
    public static class LevelEditorUtil
    {
        public const string SceneSavePath = "Assets/Scenes/Level";
        public const string SceneTemplatePath = "Assets/Scenes/Template/LevelTemplate.scenetemplate";
        public const string PrefabImportPath = "Assets/Prefabs/StageObject/";

        private static Dictionary<string, VisualTreeAsset> cachedElements;

        public static T LoadUIElement<T>(string uiName) where T : VisualElement
        {
            cachedElements ??= new Dictionary<string, VisualTreeAsset>();

            if (!cachedElements.ContainsKey(uiName))
            {
                var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"Assets/UI/{uiName}.uxml");
                cachedElements.Add(uiName, uxml);
            }

            return cachedElements[uiName].CloneTree().Q<T>(uiName);
        }

        public static string GetSceneAssetPath(string assetName)
        {
            return $"{SceneSavePath}/{assetName}.unity";
        }
    }
}