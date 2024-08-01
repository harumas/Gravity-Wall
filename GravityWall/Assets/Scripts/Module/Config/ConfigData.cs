using System;
using R3;
using UnityEngine;

namespace Module.Config
{
    [Serializable]
    public class ConfigData
    {
        public ReadOnlyReactiveProperty<Vector2> MouseSensibility => mouseSensibility;
        public ReadOnlyReactiveProperty<Vector2> PadSensibility => padSensibility;
        
        [SerializeField] SerializableReactiveProperty<Vector2> mouseSensibility = new(Vector2.one);
        [SerializeField] SerializableReactiveProperty<Vector2> padSensibility = new(Vector2.one);
    }
}