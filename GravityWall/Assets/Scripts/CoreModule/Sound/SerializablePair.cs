using System;
using System.Collections.Generic;

namespace CoreModule.Sound
{
    /// <summary>
    /// KeyValuePairのシリアライズ可能版
    /// </summary>
    [Serializable]
    public class SerializablePair<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;

        public SerializablePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public SerializablePair(KeyValuePair<TKey, TValue> pair)
        {
            Key = pair.Key;
            Value = pair.Value;
        }
    }
}