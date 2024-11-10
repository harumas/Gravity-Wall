﻿using System;
using CoreModule.Save;
using R3;
using UnityEngine;
using UnityEngine.Serialization;

namespace Module.Config
{
    [Serializable]
    public class ConfigData : ICloneable<ConfigData>
    {
        public SerializableReactiveProperty<Vector2> MouseSensibility;
        public SerializableReactiveProperty<Vector2> PadSensibility;
        public SerializableReactiveProperty<float> MasterVolume;
        public SerializableReactiveProperty<float> BgmVolume;
        public SerializableReactiveProperty<float> SeVolume;
        public SerializableReactiveProperty<float> AmbientVolume;
        public SerializableReactiveProperty<bool> Vibration;

        public ConfigData Clone()
        {
            ConfigData configData = new ConfigData
            {
                MouseSensibility = new SerializableReactiveProperty<Vector2>(MouseSensibility.CurrentValue),
                PadSensibility = new SerializableReactiveProperty<Vector2>(PadSensibility.CurrentValue),
                MasterVolume = new SerializableReactiveProperty<float>(MasterVolume.CurrentValue),
                BgmVolume = new SerializableReactiveProperty<float>(BgmVolume.CurrentValue),
                SeVolume = new SerializableReactiveProperty<float>(SeVolume.CurrentValue),
                AmbientVolume = new SerializableReactiveProperty<float>(AmbientVolume.CurrentValue),
                Vibration = new SerializableReactiveProperty<bool>(Vibration.CurrentValue)
            };

            return configData;
        }
    }
}