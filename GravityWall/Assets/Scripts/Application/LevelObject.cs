using System;
using System.Collections.Generic;
using UnityEngine;

namespace Application
{
    public class LevelObject : MonoBehaviour
    {
        public T[] GetComponentsInChildrenNonAlloc<T>(bool includeInactive) where T : Component
        {
            return transform.GetComponentsInChildren<T>(includeInactive);
        }
    }
}