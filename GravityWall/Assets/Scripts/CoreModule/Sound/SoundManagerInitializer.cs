using UnityEngine;

namespace CoreModule.Sound
{
    /// <summary>
    /// SoundManagerをランタイムで生成するクラス
    /// </summary>
    internal static class SoundManagerInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            // サウンド設定を読み込む
            SoundSettings soundSettings = Resources.Load<SoundSettings>("SoundSettings");

            if (soundSettings == null)
            {
                Debug.LogError($"{nameof(SoundSettings)}をResourcesフォルダに配置してください");
                return;
            }

            // SoundManagerのオブジェクトを生成して初期化
            GameObject gameObject = new GameObject("SoundManager");
            SoundManager soundManager = gameObject.AddComponent<SoundManager>();
            soundManager.Construct(soundSettings);
        }
    }
}