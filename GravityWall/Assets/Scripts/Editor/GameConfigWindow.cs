using Container;
using CoreModule.Save;
using Module.Config;
using TriInspector;
using UnityEditor;
using UnityEngine;
using VContainer;

namespace Editor
{
    /// <summary>
    /// ゲーム設定をエディタ上で行うクラス
    /// </summary>
    public class GameConfigWindow : EditorWindow
    {
        private SerializedObject serializedObject;
        [SerializeField] private bool lockStartScene = true;
        [SerializeField] private ConfigData configData;
        private SaveManager<ConfigData> saveManager;

        [MenuItem("Tools/GameConfigWindow", false, -100)]
        private static void Init()
        {
            GameConfigWindow window = GetWindow<GameConfigWindow>();
            window.lockStartScene = EditorPrefs.GetBool("LockStartScene", true);
            window.serializedObject = new SerializedObject(window);

            if (!UnityEngine.Application.isPlaying)
            {
                return;
            }

            window.saveManager = ExternalAccessor.Resolver.Resolve<SaveManager<ConfigData>>();
            window.configData = window.saveManager.Data;
            window.Show();
        }

        private void OnGUI()
        {
            serializedObject.Update();
            
            SerializedProperty lockStartSceneProperty = serializedObject.FindProperty(nameof(lockStartScene));
            EditorGUILayout.PropertyField(lockStartSceneProperty);

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
            EditorPrefs.SetBool("LockStartScene", lockStartScene);
            
            if (!UnityEngine.Application.isPlaying)
            {
                Debug.LogError("コンフィグは保存されませんでした。");
                return;
            }
            
            await saveManager.Save();
            Debug.Log("ゲームコンフィグを保存しました。");
        }
    }
}