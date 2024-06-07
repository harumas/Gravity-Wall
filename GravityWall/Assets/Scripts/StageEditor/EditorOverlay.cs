using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace StageEditor
{
    [Overlay(typeof(SceneView), "Editor", true, defaultDockZone = DockZone.RightToolbar)]
    public class EditorOverlay : Overlay
    {
        private Dictionary<string, VisualTreeAsset> cachedElements;
        private ObjectPlacer objectPlacer = new ObjectPlacer();

        public override VisualElement CreatePanelContent()
        {
            var root = new VisualElement();

            Foldout foldout = LoadUIElement<Foldout>("Foldout");
            foldout.text = "ProBuilder";
            root.Add(foldout);

            //New Shapeのボタンを追加
            MethodInfo newShapeMethod = GetProBuilderFunction("MenuPerform_NewShapeToggle");
            Button newShapeButton = new Button(() => newShapeMethod.Invoke(null, null));
            newShapeButton.text = "New Shape";

            foldout.Add(newShapeButton);

            //New Poly Shapeのボタンを追加
            MethodInfo newPolyMethod = GetProBuilderFunction("MenuPerform_NewPolyShapeToggle");
            Button newPolyButton = new Button(() => newPolyMethod.Invoke(null, null));
            newPolyButton.text = "New PolyShape";

            foldout.Add(newPolyButton);

            CreatePreviewButtons(root);

            Resources.UnloadUnusedAssets();

            return root;
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
    }
}