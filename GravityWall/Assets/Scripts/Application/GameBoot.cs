using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Application
{
    /// <summary>
    /// 起動時の初期シーンのロードクラス
    /// </summary>
    public static class GameBoot
    {
        public static bool IsBooted { get; private set; } = false;
        
        private static string mainScene;
        private const string BootSceneName = "Root";
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Boot()
        {
            mainScene = SceneManager.GetActiveScene().name;

            //強制的に初期シーンに遷移する
            if (mainScene != BootSceneName)
            {
                SceneManager.LoadScene(BootSceneName);
            }
        }
        
        /// <summary>
        /// メインシーンをロードします
        /// </summary>
        /// <param name="cancellationToken"></param>
        public static async UniTask LoadMainSceneAsync(CancellationToken cancellationToken)
        {
            IsBooted = true;
            
            await SceneManager.LoadSceneAsync(mainScene).ToUniTask(cancellationToken: cancellationToken);
        }
    }
}