using System.Threading;
using Cysharp.Threading.Tasks;
using Module.Config;
using Module.Core.Save;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace GameMain
{
    public class GameStarter : IAsyncStartable
    {
        public async UniTask StartAsync(CancellationToken cancellation = new CancellationToken())
        {
            //ゲームコンフィグのロード
            await SaveManager<ConfigData>.Load();
            
            //タイトルシーンのロード
            await GameBoot.LoadMainSceneAsync(cancellation);
        }
    }
}