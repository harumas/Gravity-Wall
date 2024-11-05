using System.Collections.Generic;
using UnityEngine;

namespace Module.Gimmick
{
    public class GimmickReference : MonoBehaviour
    {
        private readonly Dictionary<string, GimmickObject> gimmickObjects = new Dictionary<string, GimmickObject>();

        public void UpdateReference()
        {
            
        }

        public static bool TryGetGimmick<T>() where T : GimmickObject { }
    }
}