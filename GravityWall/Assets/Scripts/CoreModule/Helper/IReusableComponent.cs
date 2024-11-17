using System.Collections.Generic;
using UnityEngine;

namespace CoreModule.Helper
{
    public interface IReusableComponent
    {
        void SetComponentsInChildren(IEnumerable<GameObject> parents);
    }
}