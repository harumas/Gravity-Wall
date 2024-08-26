using UnityEngine;

namespace Module.Config
{
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