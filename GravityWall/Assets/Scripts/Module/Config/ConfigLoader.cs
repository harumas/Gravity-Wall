using System.Threading;
using CoreModule.Save;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Module.Config
{
    public class ConfigLoader
    {
        private readonly SaveManager<ConfigData> saveManager;

        public ConfigLoader(SaveManager<ConfigData> saveManager)
        {
            this.saveManager = saveManager;
        }
        
        public async UniTask<bool> Load(CancellationToken cancellationToken)
        {
            //デフォルトコンフィグのロード
            ResourceRequest resourceRequest = Resources.LoadAsync<DefaultConfigData>("DefaultConfigData");
            await resourceRequest.ToUniTask(cancellationToken: cancellationToken);

            DefaultConfigData defaultConfig = resourceRequest.asset as DefaultConfigData;

            if (defaultConfig == null)
            {
                Debug.LogError("デフォルト設定のロードに失敗しました。");
                return false;
            }

            //保存されたコンフィグのロード
            ConfigData configData = await LoadConfigData(defaultConfig.GetData());
            saveManager.Initialize(configData);

            return true;
        }

        private async UniTask<ConfigData> LoadConfigData(ConfigData defaultData)
        {
            ConfigData configData;
            string name = nameof(ConfigData);

            if (SaveUtility.FileExists(name))
            {
                configData = await SaveUtility.Load<ConfigData>(name);
            }
            else
            {
                configData = defaultData.Clone();
            }

            return configData;
        }
    }
}