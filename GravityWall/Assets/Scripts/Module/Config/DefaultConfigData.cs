using UnityEngine;

namespace Module.Config
{
    /// <summary>
    /// ゲームのデフォルト設定を保持するクラス
    /// </summary>
    [CreateAssetMenu(menuName = "DefaultConfigData", fileName = "DefaultConfigData")]
    public class DefaultConfigData : ScriptableObject
    {
        [SerializeField] private ConfigData configData;

        public ConfigData GetData()
        {
            return configData;
        }
    }
}