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

            if (!ValidateLevelName(levelNameField.text))
            {
                return;
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

        private bool ValidateLevelName(string name)
        {
            if (name == string.Empty)
            {
                Debug.LogError("Level名が設定されていないため、作成に失敗しました。");
                return false;
            }

            string[] names = name.Split('/');

            if (names.Length != 2)
            {
                Debug.LogError("不正なステージ名です。");
                return false;
            }

            return true;
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

            Scene currentScene = SceneManager.GetActiveScene();

            //全てのシーンアセットを取得
            foreach (string rootPath in Directory.GetDirectories(LevelEditorUtil.SceneSavePath))
            {
                List<SceneAsset> assets = LevelEditorUtil.LoadAllAsset<SceneAsset>(rootPath);

                //ディレクトリ名を取得して、それをサブメニューとする
                string rootName = Path.GetFileName(rootPath);

                foreach (SceneAsset scene in assets)
                {
                    bool enableItem = scene.name == currentScene.name;
                    string fileName = $"{rootName}/{scene.name}";
                    menu.AddItem(new GUIContent(fileName), enableItem, () => OnDropdownItemSelected(fileName));
                }
            }

            menu.ShowAsContext();
        }

        private void OnDropdownItemSelected(string itemName)
        {
            //現在のシーンをセーブ
            Scene currentScene = SceneManager.GetActiveScene();
            string sceneName = currentScene.name;

            string sceneAssetPath = AssetDatabase.FindAssets("t:Scene", new string[] { LevelEditorUtil.SceneSavePath })
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .First(path =>
                {
                    string fileName = Path.GetFileName(path);
                    
                    //.unityを削除する
                    var slicedName = fileName.Substring(0, fileName.Length - 6);
                    
                    //シーン名とアセット名が等しかったらそのパスを返す
                    return slicedName == sceneName;
                });

            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), sceneAssetPath);

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