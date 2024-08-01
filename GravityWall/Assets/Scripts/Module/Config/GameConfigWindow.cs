using Module.Core.Save;
using TriInspector;
using UnityEditor;
using UnityEngine;

namespace Module.Config
{
    /// <summary>
    /// ゲーム設定をエディタ上で行うクラス
    /// </summary>
    public class GameConfigWindow : EditorWindow
    {
        private SerializedObject serializedObject;
        [SerializeField] private ConfigData configData;

        [MenuItem("Tools/GameConfigWindow", false, -100)]
        private static void Init()
        {
            if (!Application.isPlaying)
            {
                Debug.LogError("再生中のみ表示できます。");
                return;
            }

            GameConfigWindow window = GetWindow<GameConfigWindow>();
            window.configData = SaveManager<ConfigData>.Instance;
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

            await SaveManager<ConfigData>.Save();
            Debug.Log("ゲームコンフィグを保存しました。");
        }
    }
}