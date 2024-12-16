using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CoreModule.Save
{
    /// <summary>
    /// セーブ機能を管理するクラス
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SaveManager<T> where T : ICloneable<T>
    {
        public T Data { get; private set; }
        private T defaultSaveData;

        public void Initialize(T data, T defaultSaveData)
        {
            if (Data != null)
            {
                Debug.LogError("SaveManagerは初期化済みです");
                return;
            }

            Data = data;
            this.defaultSaveData = defaultSaveData;
        }

        /// <summary>
        /// ロード処理。ファイルがなかった場合は、SaveDataクラスの初期値を使う
        /// </summary>
        public void Reset()
        {
            string name = typeof(T).Name;
            SaveUtility.Delete(name);
            Data = defaultSaveData.Clone();
        }

        /// <summary>
        /// Instanceのセーブ処理
        /// </summary>
        public async UniTask Save()
        {
            string name = typeof(T).Name;
            await SaveUtility.Save(Data, name, true);
        }
    }
}