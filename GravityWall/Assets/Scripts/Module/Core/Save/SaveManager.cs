using Core.Save;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Module.Core.Save
{
    public static class SaveManager<T> where T : ScriptableObject
    {
        public static T Instance;

        /// <summary>
        /// ロード処理。ファイルがなかった場合は、SaveDataクラスの初期値を使う
        /// </summary>
        public static async UniTask Load()
        {
            string name = typeof(T).Name;

            Instance = ScriptableObject.CreateInstance<T>();

            if (SaveUtility.FileExists(name))
            {
                await SaveUtility.LoadOverwrite(name, Instance);
            }
            else
            {
                Instance = Resources.Load<T>($"Default{name}");
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