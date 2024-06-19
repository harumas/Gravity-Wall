using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace StageEditor
{
    [Overlay(typeof(SceneView), "LevelSelector")]
    public class LevelSelectorToolbar : ToolbarOverlay
    {
        private LevelSelectorToolbar() : base(
            LevelSelectorButton.ID,
            LevelSelectorDropdown.ID
        ) { }
    }


    [EditorToolbarElement(ID, typeof(SceneView))]
    internal class LevelSelectorButton : EditorToolbarButton
    {
        public const string ID = "LevelSelectorButton";
        private string sceneSavePath = "Assets/Scenes/Level";
        public static event Action<Scene> OnLevelCreated;

        public LevelSelectorButton()
        {
            text = "Create Level";
            clicked += CreateStageButtons;
        }

        private void CreateStageButtons()
        {
            Scene currentLevel = LevelEditorUtil.GetCurrentLevel();
            if (currentLevel.IsValid())
            {
                EditorSceneManager.CloseScene(currentLevel, true);
            }

            Scene activeScene = SceneManager.GetActiveScene();
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            newScene.name = $"Level_{newScene.GetHashCode()}";
            EditorSceneManager.SaveScene(newScene, GetSceneAssetPath(newScene.name));


            if (currentLevel.IsValid())
            {
                SceneManager.SetActiveScene(activeScene);
            }

            OnLevelCreated?.Invoke(newScene);
        }

        private string GetSceneAssetPath(string assetName)
        {
            return $"{sceneSavePath}/{assetName}.unity";
        }
    }

    [EditorToolbarElement(ID, typeof(SceneView))]
    internal class LevelSelectorDropdown : EditorToolbarDropdown
    {
        public const string ID = "LevelSelectorDropdown";
        private string sceneSavePath = "Assets/Scenes/Level";
        public static event Action<Scene> OnLevelChanged;

        public LevelSelectorDropdown()
        {
            text = LevelEditorUtil.GetCurrentLevel().name;
            clicked += ShowDropdown;
        }

        private static List<T> LoadAllAsset<T>(string directoryPath) where T : Object
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

        private string GetSceneAssetPath(string assetName)
        {
            return $"{sceneSavePath}/{assetName}.unity";
        }

        void ShowDropdown()
        {
            var menu = new GenericMenu();

            List<SceneAsset> assets = LoadAllAsset<SceneAsset>(sceneSavePath);
            Scene current = LevelEditorUtil.GetCurrentLevel();

            foreach (var asset in assets.Where(asset => asset.name.Contains("Level_")))
            {
                menu.AddItem(new GUIContent(asset.name), asset.name == current.name, () => OnDropdownItemSelected(asset.name));
            }

            menu.ShowAsContext();
        }

        private void OnDropdownItemSelected(string itemName)
        {
            Scene current = LevelEditorUtil.GetCurrentLevel();

            if (current.name == itemName)
            {
                return;
            }

            if (current.IsValid())
            {
                EditorSceneManager.CloseScene(current, true);
            }

            Scene activeScene = SceneManager.GetActiveScene();
            Scene scene = EditorSceneManager.OpenScene(GetSceneAssetPath(itemName), OpenSceneMode.Additive);

            text = scene.name;

            if (activeScene.IsValid())
            {
                SceneManager.SetActiveScene(activeScene);
                OnLevelChanged?.Invoke(scene);
            }
        }
    }
}