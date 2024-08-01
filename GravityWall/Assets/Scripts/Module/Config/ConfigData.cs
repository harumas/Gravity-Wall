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
            ConfigData configData = new ConfigData
            {
                mouseSensibility = new SerializableReactiveProperty<Vector2>(mouseSensibility.CurrentValue),
                padSensibility = new SerializableReactiveProperty<Vector2>(padSensibility.CurrentValue)
            };

            return configData;
        }
    }
}