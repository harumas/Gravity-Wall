using System;
using CoreModule.Save;
using R3;
using UnityEngine;

namespace Module.Config
{
    [Serializable]
    public class ConfigData : ICloneable<ConfigData>
    {
        public ReadOnlyReactiveProperty<Vector2> MouseSensibility => mouseSensibility;
        public ReadOnlyReactiveProperty<Vector2> PadSensibility => padSensibility;
        public ReadOnlyReactiveProperty<float> MasterVolume => masterVolume;
        public ReadOnlyReactiveProperty<float> BgmVolume => bgmVolume;
        public ReadOnlyReactiveProperty<float> SeVolume => seVolume;
        public ReadOnlyReactiveProperty<float> AmbientVolume => ambientVolume;
        
        [SerializeField] private SerializableReactiveProperty<Vector2> mouseSensibility;
        [SerializeField] private SerializableReactiveProperty<Vector2> padSensibility;
        [SerializeField] private SerializableReactiveProperty<float> masterVolume;
        [SerializeField] private SerializableReactiveProperty<float> bgmVolume;
        [SerializeField] private SerializableReactiveProperty<float> seVolume;
        [SerializeField] private SerializableReactiveProperty<float> ambientVolume;

        public ConfigData Clone()
        {
            ConfigData configData = new ConfigData
            {
                mouseSensibility = new SerializableReactiveProperty<Vector2>(mouseSensibility.CurrentValue),
                padSensibility = new SerializableReactiveProperty<Vector2>(padSensibility.CurrentValue),
                masterVolume = new SerializableReactiveProperty<float>(masterVolume.CurrentValue),
                bgmVolume = new SerializableReactiveProperty<float>(bgmVolume.CurrentValue),
                seVolume = new SerializableReactiveProperty<float>(seVolume.CurrentValue),
                ambientVolume = new SerializableReactiveProperty<float>(ambientVolume.CurrentValue)
            };

            return configData;
        }
    }
}