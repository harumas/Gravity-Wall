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
        private readonly ObjectPlacer objectPlacer = new ObjectPlacer();
        private readonly VisualElement root = new VisualElement();

        public override VisualElement CreatePanelContent()
        {
            root.Clear();

            //ObjectPlacerの対象シーンの更新イベントの登録
            LevelSelectorDropdown.OnLevelChanged -= objectPlacer.SetNewScene;
            LevelSelectorDropdown.OnLevelChanged += objectPlacer.SetNewScene;
            LevelSelectorButton.OnLevelCreated -= objectPlacer.SetNewScene;
            LevelSelectorButton.OnLevelCreated += objectPlacer.SetNewScene;

            //オーバーレイのリフレッシュイベントの登録
            EditorApplication.playModeStateChanged -= OnPlayerModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayerModeStateChanged;
            EditorSceneManager.sceneOpened += OnSceneOpened;

            CreatePreviewButtons(root);

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
            //Prefabのフォルダを取得する
            string[] directories = Directory.GetDirectories(LevelEditorUtil.PrefabImportPath);

            foreach (string directoryPath in directories)
            {
                //フォルダ名で折り畳めるセクションを作成する
                string fileName = directoryPath.Split('/')[3];
                Foldout foldout = LevelEditorUtil.LoadUIElement<Foldout>("Foldout");
                foldout.text = fileName;
                root.Add(foldout);

                //横列のグループを作成する
                GroupBox previewGroup = foldout.Q<GroupBox>("PreviewGroup");

                //Prefabからサムネイル画像を生成する
                string[] files = Directory.GetFiles(directoryPath, "*.prefab", SearchOption.AllDirectories);
                GameObject[] objects = files.Select(AssetDatabase.LoadAssetAtPath<GameObject>).ToArray();
                Texture2D[] thumbnails = CaptureCreator.GetThumbnails(objects);

                for (var i = 0; i < objects.Length; i++)
                {
                    GameObject prefab = objects[i];
                    Texture2D thumbnail = thumbnails[i];

                    Button previewButton = CreatePreviewButton(prefab, thumbnail);

                    previewGroup.Add(previewButton);
                }
            }

            Resources.UnloadUnusedAssets();
        }

        private Button CreatePreviewButton(GameObject prefab, Texture2D thumbnail)
        {
            Button button = LevelEditorUtil.LoadUIElement<Button>("ObjectElement");

            //ボタンの背景をプレビューアイコンに設定
            StyleBackground background = button.style.backgroundImage;
            Background value = background.value;
            value.texture = thumbnail;
            background.value = value;
            button.style.backgroundImage = background;

            button.clicked += () => { objectPlacer.PlaceObject(prefab); };

            return button;
        }
    }
}