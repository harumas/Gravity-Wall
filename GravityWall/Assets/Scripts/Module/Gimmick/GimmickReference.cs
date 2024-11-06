using System.Collections.Generic;
using UnityEngine;

namespace Module.Gimmick
{
    public class GimmickReference : MonoBehaviour
    {
        private static readonly Dictionary<string, GimmickObject> gimmickObjects = new Dictionary<string, GimmickObject>();

        public void UpdateReference()
        {
            GimmickObject[] gimmicks = FindObjectsByType<GimmickObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            gimmickObjects.Clear();

            foreach (GimmickObject gimmick in gimmicks)
            {
                gimmickObjects.Add(gimmick.Path, gimmick);
            }
        }

        public static bool TryGetGimmick<T>(string gimmickPath, out T gimmick) where T : GimmickObject
        {
            if (gimmickObjects.TryGetValue(gimmickPath, out GimmickObject gimmickObject) &&
                gimmickObject is T item)
            {
                gimmick = item;
                return true;
            }

            gimmick = null;
            return false;
        }
        
    }
}