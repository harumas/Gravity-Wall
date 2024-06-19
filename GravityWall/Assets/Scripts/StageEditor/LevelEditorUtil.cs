using System.Collections.Generic;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace StageEditor
{
    public static class LevelEditorUtil
    {
        private static Dictionary<string, VisualTreeAsset> cachedElements;

        public static Scene GetCurrentLevel()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (scene.name.Contains("Level_"))
                {
                    return scene;
                }
            }

            return default;
        }

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
    }
}