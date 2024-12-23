using CoreModule.Helper.Attribute;
using Module.Config;
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
        private static bool lockStartScene;
        private const string BootSceneName = "Root";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Boot()
        {
            startScene = SceneManager.GetActiveScene().name;

#if UNITY_EDITOR
            lockStartScene = EditorPrefs.GetBool("LockStartScene", true);
#endif

            //強制的に初期シーンに遷移する
            if (startScene != BootSceneName)
            {
                SceneManager.LoadScene(BootSceneName);
            }
        }

        /// <summary>
        /// メインシーンをロードします
        /// </summary>
        /// <param name="sceneGroup"></param>
        public static void LoadRootScene(SceneGroup sceneGroup)
        {
            IsBooted = true;

            if (lockStartScene)
            {
                SceneManager.LoadScene(startScene, LoadSceneMode.Single);
            }
            else
            {
                int loadCount = 0;

                foreach (SceneField sceneField in sceneGroup.GetScenes())
                {
                    LoadSceneMode mode = loadCount == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive;
                    SceneManager.LoadScene(sceneField, mode);
                    loadCount++;
                }
            }
        }
    }
}