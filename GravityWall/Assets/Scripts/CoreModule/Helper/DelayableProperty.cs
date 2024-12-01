using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CoreModule.Helper
{
    public class DelayableProperty<T> where T : IEquatable<T>
    {
        public T Value { get; private set; }

        private T nextValue;
        private float delay;
        private float elapsedTime;
        private bool enabled;

        public void Assign(T value, float delay)
        {
            if (nextValue.Equals(value))
            {
                return;
            }

            nextValue = value;
            this.delay = delay;
            elapsedTime = 0f;
            enabled = true;
        }

        public void Update()
        {
            if (!enabled)
            {
                return;
            }
            
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= delay)
            {
                Value = nextValue;
                enabled = false;
            }
        }
    }
}