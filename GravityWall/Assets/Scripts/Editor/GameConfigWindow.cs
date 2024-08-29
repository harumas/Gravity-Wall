using Container;
using CoreModule.Save;
using TriInspector;
using UnityEditor;
using UnityEngine;
using VContainer;

namespace Module.Config
{
    /// <summary>
    /// ゲーム設定をエディタ上で行うクラス
    /// </summary>
    public class GameConfigWindow : EditorWindow
    {
        private SerializedObject serializedObject;
        [SerializeField] private ConfigData configData;
        private SaveManager<ConfigData> saveManager;

        [MenuItem("Tools/GameConfigWindow", false, -100)]
        private static void Init()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("再生中のみ表示できます。");
                return;
            }

            GameConfigWindow window = GetWindow<GameConfigWindow>();
            window.saveManager = ExternalAccessor.Resolver.Resolve<SaveManager<ConfigData>>();
            window.configData = window.saveManager.Data;
            window.serializedObject = new SerializedObject(window);
            window.Show();
        }

        private void OnGUI()
        {
            serializedObject.Update();

            //ConfigDataクラス
            SerializedProperty serializedProperty = serializedObject.FindProperty(nameof(configData));
            EditorGUILayout.PropertyField(serializedProperty);

            //セーブボタン
            if (GUILayout.Button("Save", GUILayout.Height((float)ButtonSizes.Large)))
            {
                Save();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private async void Save()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("再生中のみ変更できます");
                return;
            }

            await saveManager.Save();
            Debug.Log("ゲームコンフィグを保存しました。");
        }
    }
}