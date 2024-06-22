using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEditor.SceneTemplate;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace StageEditor
{
    [Overlay(typeof(SceneView), "LevelSelector")]
    public class LevelSelectorToolbar : ToolbarOverlay
    {
        private LevelSelectorToolbar() : base(
            LevelSelectorButton.ID,
            LevelSelectorDropdown.ID
        )
        {
        }
    }

    [EditorToolbarElement(ID, typeof(SceneView))]
    internal class LevelSelectorButton : EditorToolbarButton
    {
        public const string ID = "LevelSelectorButton";

        public static event Action<Scene> OnLevelCreated;

        public LevelSelectorButton()
        {
            TextField levelNameField = LevelEditorUtil.LoadUIElement<TextField>("LevelNameField");
            Add(levelNameField);

            text = "Create Level";
            clicked += () => CreateStageButtons(levelNameField);
        }

        private void CreateStageButtons(TextField levelNameField)
        {
            Scene currentLevel = SceneManager.GetActiveScene();
            
            if (currentLevel.IsValid())
            {
                EditorSceneManager.CloseScene(currentLevel, true);
            }

            SceneTemplateAsset template = AssetDatabase.LoadAssetAtPath<SceneTemplateAsset>(LevelEditorUtil.SceneTemplatePath);
            string assetPath = LevelEditorUtil.GetSceneAssetPath(levelNameField.text);
            InstantiationResult result = SceneTemplateService.Instantiate(template, false, assetPath);
            levelNameField.SetValueWithoutNotify(null);
            
            Scene newScene = result.scene;
            OnLevelCreated?.Invoke(newScene);
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
            text = SceneManager.GetActiveScene().name;
            LevelSelectorButton.OnLevelCreated += scene => text = scene.name;
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

        private void ShowDropdown()
        {
            var menu = new GenericMenu();

            List<SceneAsset> assets = LoadAllAsset<SceneAsset>(sceneSavePath);
            Scene current = SceneManager.GetActiveScene();

            foreach (var asset in assets)
            {
                menu.AddItem(new GUIContent(asset.name), asset.name == current.name, () => OnDropdownItemSelected(asset.name));
            }

            menu.ShowAsContext();
        }

        private void OnDropdownItemSelected(string itemName)
        {
            string assetPath = LevelEditorUtil.GetSceneAssetPath(itemName);
            Scene scene = EditorSceneManager.OpenScene(assetPath, OpenSceneMode.Single);
            text = scene.name;
            OnLevelChanged?.Invoke(scene);
        }
    }
}