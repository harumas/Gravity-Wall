using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace StageEditor
{
    [Overlay(typeof(SceneView), "Level Editor", true, defaultDockZone = DockZone.RightToolbar)]
    public class EditorOverlay : Overlay
    {
        private Dictionary<string, VisualTreeAsset> cachedElements;
        private ObjectPlacer objectPlacer = new ObjectPlacer();
        private VisualElement root = new VisualElement();
        private string sceneSavePath = "Assets/Scenes/Level";

        public override VisualElement CreatePanelContent()
        {
            root = new VisualElement();

            CreateStageButtons(root);
            CreateLevelsDropdown(root);
            CreateProBuilderButtons(root);
            CreatePreviewButtons(root);

            EditorApplication.playModeStateChanged -= OnPlayerModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayerModeStateChanged;

            EditorSceneManager.sceneOpened += OnSceneOpened;

            objectPlacer.Initialize();
            objectPlacer.SetNewScene(GetCurrentLevel());

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
            CreateStageButtons(root);
            CreateLevelsDropdown(root);
            CreateProBuilderButtons(root);
            CreatePreviewButtons(root);

            objectPlacer.SetNewScene(GetCurrentLevel());
        }

        private void CreateStageButtons(VisualElement root)
        {
            Button button = LoadUIElement<Button>("LargeButton");
            button.text = "Create Level";

            button.clicked += () =>
            {
                Scene currentLevel = GetCurrentLevel();
                if (currentLevel.IsValid())
                {
                    EditorSceneManager.CloseScene(currentLevel, true);
                }

                Scene activeScene = SceneManager.GetActiveScene();
                Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                newScene.name = $"Level_{newScene.GetHashCode()}";
                EditorSceneManager.SaveScene(newScene, $"{sceneSavePath}/{newScene.name}.unity");
                SceneManager.SetActiveScene(activeScene);

                objectPlacer.SetNewScene(newScene);

                RefreshLevelsDropdown();
            };

            root.Add(button);
        }

        private void CreateLevelsDropdown(VisualElement root)
        {
            DropdownField levelField = new DropdownField("Levels");
            levelField.name = "Levels";
            levelField.RegisterValueChangedCallback(OnLevelSelected);

            root.Add(levelField);

            RefreshLevelsDropdown();
        }

        private void OnLevelSelected(ChangeEvent<string> evt)
        {
            Scene current = GetCurrentLevel();

            if (current.name == evt.newValue)
            {
                return;
            }
            
            if (current.IsValid())
            {
                EditorSceneManager.CloseScene(current, true);
            }

            Scene activeScene = SceneManager.GetActiveScene();
            Scene scene = EditorSceneManager.OpenScene($"{sceneSavePath}/{evt.newValue}.unity", OpenSceneMode.Additive);

            if (scene.IsValid())
            {
                SceneManager.SetActiveScene(activeScene);
                objectPlacer.SetNewScene(scene);
            }
            else
            {
                RefreshLevelsDropdown();
            }
        }

        private void RefreshLevelsDropdown()
        {
            DropdownField levelField = root.Q<DropdownField>();
            levelField.choices.Clear();

            List<SceneAsset> assets = LoadAll<SceneAsset>(sceneSavePath);

            foreach (var asset in assets.Where(asset => asset.name.Contains("Level_")))
            {
                levelField.choices.Add(asset.name);
            }

            Scene scene = GetCurrentLevel();
            if (scene.IsValid())
            {
                string sceneName = scene.name;
                levelField.index = levelField.choices.FindIndex(choice => choice == sceneName);
            }
            else
            {
                levelField.index = -1;
            }
        }

        private Scene GetCurrentLevel()
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

        private void CreateProBuilderButtons(VisualElement root)
        {
            Foldout foldout = LoadUIElement<Foldout>("Foldout");
            foldout.text = "ProBuilder";
            root.Add(foldout);

            //New Shapeのボタンを追加
            MethodInfo newShapeMethod = GetProBuilderFunction("MenuPerform_NewShapeToggle");
            Button newShapeButton = new Button(() =>
            {
                OpenProBuilderWindow();
                newShapeMethod.Invoke(null, null);
            });
            newShapeButton.text = "New Shape";

            foldout.Add(newShapeButton);

            //New Poly Shapeのボタンを追加
            MethodInfo newPolyMethod = GetProBuilderFunction("MenuPerform_NewPolyShapeToggle");
            Button newPolyButton = new Button(() =>
            {
                OpenProBuilderWindow();
                newPolyMethod.Invoke(null, null);
            });
            newPolyButton.text = "New PolyShape";

            foldout.Add(newPolyButton);
        }

        private void CreatePreviewButtons(VisualElement root)
        {
            const string importPath = @"Assets\Prefabs\StageObject\";

            //Prefabのフォルダを取得する
            string[] directories = Directory.GetDirectories(importPath);

            foreach (string directory in directories)
            {
                //フォルダ名で折り畳めるセクションを作成する
                Foldout foldout = LoadUIElement<Foldout>("Foldout");
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

                    Button button = LoadUIElement<Button>("ObjectElement");

                    //ボタンの背景をプレビューアイコンに設定
                    StyleBackground background = button.style.backgroundImage;
                    Background value = background.value;
                    value.texture = thumbnail;
                    background.value = value;
                    button.style.backgroundImage = background;

                    button.clicked += () => { objectPlacer.PlaceObject(obj); };

                    previewGroup.Add(button);
                }
            }

            Resources.UnloadUnusedAssets();
        }

        private T LoadUIElement<T>(string uiName) where T : VisualElement
        {
            cachedElements ??= new Dictionary<string, VisualTreeAsset>();

            if (!cachedElements.ContainsKey(uiName))
            {
                var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"Assets/UI/{uiName}.uxml");
                cachedElements.Add(uiName, uxml);
            }

            return cachedElements[uiName].CloneTree().Q<T>(uiName);
        }

        private MethodInfo GetProBuilderFunction(string name)
        {
            Assembly assembly = Assembly.Load("Unity.ProBuilder.Editor");
            Type type = assembly.GetType("UnityEditor.ProBuilder.EditorToolbarMenuItem");
            MethodInfo methodInfo = type.GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic);

            return methodInfo;
        }

        private void OpenProBuilderWindow()
        {
            Assembly assembly = Assembly.Load("Unity.ProBuilder.Editor");
            Type type = assembly.GetType("UnityEditor.ProBuilder.ProBuilderEditor");
            MethodInfo methodInfo = type.GetMethod("MenuOpenWindow", BindingFlags.Static | BindingFlags.NonPublic);

            methodInfo.Invoke(null, null);
        }


        private static List<T> LoadAll<T>(string directoryPath) where T : Object
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