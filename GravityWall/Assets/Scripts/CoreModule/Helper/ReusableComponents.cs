using System;
using System.Collections.Generic;
using UnityEngine;

namespace CoreModule.Helper
{
    public class ReusableComponents<T> : IReusableComponent where T : Component
    {
        private readonly List<T> components;

        public ReusableComponents(int defaultCapacity = 64)
        {
            components = new List<T>(defaultCapacity);
        }

        public void SetComponentsInChildren(IEnumerable<GameObject> parents)
        {
            components.Clear();

            foreach (GameObject parent in parents)
            {
                components.AddRange(parent.GetComponentsInChildren<T>(true));
            }
        }

        public ReadOnlySpan<T> GetComponents()
        {
            return components.AsSpan();
        }
    }
}