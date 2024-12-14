using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Module.Gimmick
{
    public class GimmickReference : MonoBehaviour
    {
        private readonly Dictionary<string, GimmickObject> gimmickObjects = new Dictionary<string, GimmickObject>();
        public static event Action<GimmickReference> OnGimmickReferenceUpdated;

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        private static void Initialize()
        {
            OnGimmickReferenceUpdated = null;
        }

        private void Awake()
        {
            OnGimmickReferenceUpdated = null;
        }

        public void UpdateReference()
        {
            GimmickObject[] gimmicks = FindObjectsByType<GimmickObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            gimmickObjects.Clear();

            foreach (GimmickObject gimmick in gimmicks)
            {
                gimmickObjects.Add(gimmick.Path, gimmick);
            }

            OnGimmickReferenceUpdated?.Invoke(this);
            OnGimmickReferenceUpdated = null;
        }

        public bool TryGetGimmick<T>(string gimmickPath, out T gimmick) where T : GimmickObject
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