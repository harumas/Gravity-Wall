using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEditor.SceneTemplate;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace LevelEditor
{
    /// <summary>
    /// ステージ作成ボタン
    /// </summary>
    [EditorToolbarElement(nameof(LevelCreatorButton), typeof(SceneView))]
    internal class LevelCreatorButton : EditorToolbarButton
    {
        public static event Action<Scene> OnLevelCreated;

        public LevelCreatorButton()
        {
            //ステージ名入力欄の登録
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

            //テンプレートからシーンを生成
            SceneTemplateAsset template = AssetDatabase.LoadAssetAtPath<SceneTemplateAsset>(LevelEditorUtil.SceneTemplatePath);
            string savePath = LevelEditorUtil.GetSceneAssetPath(levelNameField.text);
            InstantiationResult result = SceneTemplateService.Instantiate(template, false, savePath);

            //入力欄の初期化
            levelNameField.SetValueWithoutNotify(null);

            Scene newScene = result.scene;
            OnLevelCreated?.Invoke(newScene);
        }
    }

    /// <summary>
    /// ステージ選択欄
    /// </summary>
    [EditorToolbarElement(nameof(LevelSelectorDropdown), typeof(SceneView))]
    internal class LevelSelectorDropdown : EditorToolbarDropdown
    {
        public static event Action<Scene> OnLevelChanged;

        public LevelSelectorDropdown()
        {
            text = SceneManager.GetActiveScene().name;
            LevelCreatorButton.OnLevelCreated += scene => text = scene.name;

            clicked += ShowDropdown;
        }

        private void ShowDropdown()
        {
            var menu = new GenericMenu();

            //全てのシーンアセットを取得
            List<SceneAsset> assets = LevelEditorUtil.LoadAllAsset<SceneAsset>(LevelEditorUtil.SceneSavePath);
            Scene currentScene = SceneManager.GetActiveScene();

            foreach (var asset in assets)
            {
                bool enableItem = asset.name == currentScene.name;
                menu.AddItem(new GUIContent(asset.name), enableItem, () => OnDropdownItemSelected(asset.name));
            }

            menu.ShowAsContext();
        }

        private void OnDropdownItemSelected(string itemName)
        {
            //現在のシーンをセーブ
            Scene currentScene = SceneManager.GetActiveScene();
            string currentAssetPath = LevelEditorUtil.GetSceneAssetPath(currentScene.name);
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), currentAssetPath);

            //シーン名からシーンをロード
            string assetPath = LevelEditorUtil.GetSceneAssetPath(itemName);
            Scene scene = EditorSceneManager.OpenScene(assetPath, OpenSceneMode.Single);
            text = scene.name;
            OnLevelChanged?.Invoke(scene);
        }
    }


    [Overlay(typeof(SceneView), "LevelToolbar")]
    public class LevelToolbar : ToolbarOverlay
    {
        private LevelToolbar() : base(
            nameof(LevelCreatorButton),
            nameof(LevelSelectorDropdown)
        ) { }
    }
}