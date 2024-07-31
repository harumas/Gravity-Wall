using System;
using R3;
using UnityEngine;

namespace Module.Config
{
    [Serializable]
    public class ConfigData
    {
        public SerializableReactiveProperty<Vector2> MouseSensibility = new(Vector2.one);
        public SerializableReactiveProperty<Vector2> PadSensibility = new(Vector2.one);
    }
}