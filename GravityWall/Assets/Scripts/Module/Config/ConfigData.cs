using System;
using Module.Core.Save;
using R3;
using UnityEngine;

namespace Module.Config
{
    [Serializable]
    public class ConfigData : ICloneable<ConfigData>
    {
        public ReadOnlyReactiveProperty<Vector2> MouseSensibility => mouseSensibility;
        public ReadOnlyReactiveProperty<Vector2> PadSensibility => padSensibility;
        
        [SerializeField] SerializableReactiveProperty<Vector2> mouseSensibility;
        [SerializeField] SerializableReactiveProperty<Vector2> padSensibility;

        public ConfigData Clone()
        {
            ConfigData configData = new ConfigData();
            
            configData.mouseSensibility = new SerializableReactiveProperty<Vector2>(configData.mouseSensibility.CurrentValue);
            configData.padSensibility = new SerializableReactiveProperty<Vector2>(configData.padSensibility.CurrentValue);

            return configData;
        }
    }
}