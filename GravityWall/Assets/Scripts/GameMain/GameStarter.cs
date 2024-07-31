using System.Threading;
using Cysharp.Threading.Tasks;
using Module.Config;
using Module.Core.Save;
using VContainer;
using VContainer.Unity;

namespace GameMain
{
    public class GameStarter : IAsyncStartable
    {
        [Inject]
        public GameStarter() { }

        public async UniTask StartAsync(CancellationToken cancellation = new CancellationToken())
        {
            await SaveManager<ConfigData>.Load();
        }
    }
}