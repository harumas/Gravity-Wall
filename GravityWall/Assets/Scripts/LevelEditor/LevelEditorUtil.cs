using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LevelEditor
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

        public static List<T> LoadAllAsset<T>(string directoryPath) where T : Object
        {
            List<T> assetList = new List<T>();

            string[] filePathArray = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);

            foreach (string filePath in filePathArray)
            {
                T asset = AssetDatabase.LoadAssetAtPath<T>(filePath);
                if (asset != null)
                {
                    assetList.Add(asset);
                }
            }

            return assetList;
        }
    }
}