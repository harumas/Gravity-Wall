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
            ResourceRequest resourceRequest = Resources.LoadAsync<DefaultConfigData>("DefaultConfigData");
            await resourceRequest.ToUniTask(cancellationToken: cancellation);
            
            DefaultConfigData defaultConfig = resourceRequest.asset as DefaultConfigData;

            if (defaultConfig == null)
            {
                Debug.LogError("デフォルト設定のロードに失敗しました。");
                return;
            }
            
            await SaveManager<ConfigData>.Load(defaultConfig.GetData());
            
            //タイトルシーンのロード
            await GameBoot.LoadMainSceneAsync(cancellation);
        }
    }
}