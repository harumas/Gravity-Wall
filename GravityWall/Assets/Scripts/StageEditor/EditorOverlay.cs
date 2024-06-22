using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace StageEditor
{
    [Overlay(typeof(SceneView), "Level Editor", true, defaultDockZone = DockZone.RightToolbar)]
    public class EditorOverlay : Overlay
    {
        private VisualElement root = new VisualElement();
        private ObjectPlacer objectPlacer = new ObjectPlacer();

        public override VisualElement CreatePanelContent()
        {
            root = new VisualElement();
            LevelSelectorDropdown.OnLevelChanged -= objectPlacer.SetNewScene;
            LevelSelectorDropdown.OnLevelChanged += objectPlacer.SetNewScene;
            
            LevelSelectorButton.OnLevelCreated -= objectPlacer.SetNewScene;
            LevelSelectorButton.OnLevelCreated += objectPlacer.SetNewScene;

            CreatePreviewButtons(root);

            EditorApplication.playModeStateChanged -= OnPlayerModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayerModeStateChanged;

            EditorSceneManager.sceneOpened += OnSceneOpened;

            objectPlacer.Initialize();
            objectPlacer.SetNewScene(SceneManager.GetActiveScene());

            return root;
        }

        private void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            RefreshPanel();
        }

        private void OnPlayerModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                RefreshPanel();
            }
        }

        private async void RefreshPanel()
        {
            await Task.Delay(100);

            root.Clear();
            CreatePreviewButtons(root);

            objectPlacer.SetNewScene(SceneManager.GetActiveScene());
        }

        private void CreatePreviewButtons(VisualElement root)
        {
            const string importPath = @"Assets\Prefabs\StageObject\";

            //Prefabのフォルダを取得する
            string[] directories = Directory.GetDirectories(importPath);

            foreach (string directory in directories)
            {
                //フォルダ名で折り畳めるセクションを作成する
                Foldout foldout = LevelEditorUtil.LoadUIElement<Foldout>("Foldout");
                foldout.text = directory.Split('\\')[3];
                root.Add(foldout);

                //横列のグループを作成する
                GroupBox previewGroup = foldout.Q<GroupBox>("PreviewGroup");

                //Prefabからサムネイル画像を生成する
                string[] files = Directory.GetFiles(directory, "*.prefab", SearchOption.AllDirectories);
                GameObject[] objects = files.Select(AssetDatabase.LoadAssetAtPath<GameObject>).Where(obj => obj != null).ToArray();
                Texture2D[] thumbnails = CaptureCreator.GetThumbnails(objects);

                for (var i = 0; i < objects.Length; i++)
                {
                    GameObject obj = objects[i];
                    Texture2D thumbnail = thumbnails[i];

                    Button button = LevelEditorUtil.LoadUIElement<Button>("ObjectElement");

                    //ボタンの背景をプレビューアイコンに設定
                    StyleBackground background = button.style.backgroundImage;
                    Background value = background.value;
                    value.texture = thumbnail;
                    background.value = value;
                    button.style.backgroundImage = background;

                    button.clicked += () =>
                    {
                        objectPlacer.PlaceObject(obj);
                    };

                    previewGroup.Add(button);
                }
            }

            Resources.UnloadUnusedAssets();
        }
    }
}