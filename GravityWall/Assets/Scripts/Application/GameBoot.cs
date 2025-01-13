using CoreModule.Helper.Attribute;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Application
{
    /// <summary>
    /// 起動時の初期シーンのロードクラス
    /// </summary>
    public static class GameBoot
    {
        public static bool IsBooted { get; private set; } = false;

        private static string startScene;
        private static bool forceStartScene;
        private static AsyncOperation rootSceneOperation;
        private const string BootSceneName = "Root";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Boot()
        {
            startScene = SceneManager.GetActiveScene().name;

            // エディタ上では初期シーンを強制的に決める
#if UNITY_EDITOR
            forceStartScene = EditorPrefs.GetBool("LockStartScene", true);
#endif

            // 強制的に初期シーンに遷移する
            if (startScene != BootSceneName)
            {
                SceneManager.LoadScene(BootSceneName);
            }
        }

        /// <summary>
        /// メインシーンをロードします
        /// </summary>
        /// <param name="sceneGroup"></param>
        public static async UniTask LoadRootScene(SceneField sceneGroup)
        {
            IsBooted = true;

            string sceneName = forceStartScene ? startScene : sceneGroup;

            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }
    }
}