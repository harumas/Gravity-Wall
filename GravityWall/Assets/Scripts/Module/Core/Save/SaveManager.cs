using Core.Save;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Module.Core.Save
{
    public static class SaveManager<T> where T : new()
    {
        public static T Instance;

        /// <summary>
        /// ロード処理。ファイルがなかった場合は、SaveDataクラスの初期値を使う
        /// </summary>
        public static async UniTask Load()
        {
            string name = typeof(T).Name;

            if (SaveUtility.FileExists(name))
            {
                Instance = await SaveUtility.Load<T>(name);
            }
            else
            {
                Instance = new T();
            }
        }

        public static void Reset()
        {
            string name = typeof(T).Name;
            SaveUtility.Delete(name);
        }

        /// <summary>
        /// Instanceのセーブ処理
        /// </summary>
        public static async UniTask Save()
        {
            string name = typeof(T).Name;
            await SaveUtility.Save(Instance, name, true);
        }
    }
}