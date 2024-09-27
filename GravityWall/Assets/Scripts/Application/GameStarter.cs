using System.Threading;
using Cysharp.Threading.Tasks;
using Module.Config;
using VContainer;
using VContainer.Unity;

namespace Application
{
    /// <summary>
    /// ゲームのロードクラス
    /// </summary>
    public class GameStarter : IAsyncStartable
    {
        private readonly ConfigLoader configLoader;
        
        [Inject]
        public GameStarter(ConfigLoader configLoader)
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
            
            //タイトルシーンのロード
            await GameBoot.LoadMainSceneAsync(cancellation);
        }
    }
}