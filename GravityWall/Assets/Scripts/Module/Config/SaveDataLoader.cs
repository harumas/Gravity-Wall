using System;
using System.Threading;
using CoreModule.Save;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Module.Config
{
    /// <summary>
    /// 設定情報の読み込みを行うクラス
    /// </summary>
    public class SaveDataLoader
    {
        private readonly SaveManager<ConfigData> configManager;
        private readonly SaveManager<SaveData> saveManager;

        public event Action<SaveData, ConfigData> OnLoaded;

        public SaveDataLoader(SaveManager<ConfigData> configManager, SaveManager<SaveData> saveManager)
        {
            this.configManager = configManager;
            this.saveManager = saveManager;
        }

        /// <summary>
        /// 設定情報の読み込みを非同期で行います
        /// </summary>
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

            //保存されたセーブデータのロード
            SaveData defaultSaveData = new SaveData();
            SaveData saveData = await LoadData(defaultSaveData);
            saveManager.Initialize(saveData);

            //保存されたコンフィグのロード
            ConfigData configData = await LoadData(defaultConfig.GetData());
            configManager.Initialize(configData);

            OnLoaded?.Invoke(saveData, configData);
            return true;
        }

        private async UniTask<T> LoadData<T>(T defaultData) where T : ICloneable<T>
        {
            T configData;
            string fileName = typeof(T).Name;

            // セーブファイルが存在したらロード
            if (SaveUtility.FileExists(fileName))
            {
                configData = await SaveUtility.Load<T>(fileName);
            }
            else
            {
                // 存在しない場合はデフォルト設定をコピーする
                configData = defaultData.Clone();
            }

            return configData;
        }
    }
}