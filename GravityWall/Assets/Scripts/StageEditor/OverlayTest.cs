using System.IO;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

[Overlay(typeof(SceneView), "Objects", true)]
public class OverlayTest : Overlay
{
    public override VisualElement CreatePanelContent()
    {
        var root = new VisualElement();

        CreatePreviewButtons(root);

        var refreshButtonUxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/RefreshButton.uxml");
        Button button = refreshButtonUxml.CloneTree().Q<Button>("Refresh");
        button.clicked += () =>
        {
            root.Clear();
            CreatePreviewButtons(root);
            root.Add(button);
        };

        root.Add(button);

        return root;
    }

    private void CreatePreviewButtons(VisualElement root)
    {
        const string importPath = @"Assets\Prefabs\StageObject\";
        var buttonUxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Button.uxml");
        var foldoutUxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Foldout.uxml");

        string[] directories = Directory.GetDirectories(importPath);

        foreach (string directory in directories)
        {
            Foldout foldout = foldoutUxml.CloneTree().Q<Foldout>("Foldout");
            foldout.text = directory.Split('\\')[3];
            root.Add(foldout);

            GroupBox previewGroup = foldout.Q<GroupBox>("PreviewGroup");

            string[] files = Directory.GetFiles(directory, "*.*");

            foreach (string file in files)
            {
                GameObject obj = AssetDatabase.LoadAssetAtPath(file, typeof(GameObject)) as GameObject;

                if (obj != null)
                {
                    var buttonElement = buttonUxml.CloneTree();
                    Button button = buttonElement.Q<Button>("ObjectElement");

                    //ボタンの背景をプレビューアイコンに設定
                    StyleBackground background = button.style.backgroundImage;
                    Background value = background.value;
                    value.texture = AssetPreview.GetAssetPreview(obj);
                    background.value = value;
                    button.style.backgroundImage = background;

                    button.clicked += () =>
                    {
                        Debug.Log(obj.name);
                    };

                    previewGroup.Add(button);
                }
            }
        }
    }
}