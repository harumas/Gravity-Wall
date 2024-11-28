using System.Threading;
using Cysharp.Threading.Tasks;
using Module.Config;
using VContainer;
using VContainer.Unity;

namespace Application
{
    /// <summary>
    /// アプリケーションの開始クラス
    /// </summary>
    public class ApplicationStarter : IAsyncStartable
    {
        private readonly ConfigLoader configLoader;
        
        [Inject]
        public ApplicationStarter(ConfigLoader configLoader)
        {
            this.configLoader = configLoader;
        }
        
        public async UniTask StartAsync(CancellationToken cancellation = new CancellationToken())
        {
            bool succeed = await configLoader.Load(cancellation);

            if (!succeed)
            {
                return;
            }
            
            // FPSの上限は120
            UnityEngine.Application.targetFrameRate = 120;
            
            // タイトルシーンのロード
            await GameBoot.LoadMainSceneAsync(cancellation);
        }
    }
}