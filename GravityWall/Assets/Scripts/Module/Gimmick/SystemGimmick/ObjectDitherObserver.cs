using System.Collections.Generic;
using Constants;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    /// <summary>
    /// カメラとプレイヤーの間のオブジェクトを透過するクラス
    /// </summary>
    public class ObjectDitherObserver : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float maxDistance;

        private const int maxBufferCount = 16;
        private readonly RaycastHit[] hitResults = new RaycastHit[maxBufferCount];
        private readonly Dictionary<GameObject, ObjectDither> hitObjects = new(maxBufferCount);
        private readonly HashSet<GameObject> containedObjects = new(maxBufferCount);
        private readonly List<GameObject> removeObjectsCache = new(maxBufferCount);
        private const int layerMask = Layer.Mask.Base | Layer.Mask.IgnoreGravity;

        private void Update()
        {
            // カメラからプレイヤーまでのオブジェクトを取得
            Ray ray = new Ray(transform.position, playerTransform.position - transform.position);
            int hitCount = Physics.RaycastNonAlloc(ray, hitResults, maxDistance, layerMask, QueryTriggerInteraction.Collide);

            // 
            for (int i = 0; i < hitCount; i++)
            {
                GameObject hitObj = hitResults[i].transform.gameObject;

                if (!hitObj.CompareTag(Tag.Dither))
                {
                    continue;
                }

                containedObjects.Add(hitObj);

                if (hitObjects.Count == hitResults.Length || hitObjects.ContainsKey(hitObj))
                {
                    continue;
                }

                var dither = hitObj.GetComponent<ObjectDither>();
                hitObjects.Add(hitObj, dither);
                dither.Show();
            }

            foreach ((GameObject obj, ObjectDither dither) in hitObjects)
            {
                if (!containedObjects.Contains(obj))
                {
                    dither.Hide();
                    removeObjectsCache.Add(obj);
                }
            }

            foreach (GameObject obj in removeObjectsCache)
            {
                hitObjects.Remove(obj);
            }

            removeObjectsCache.Clear();
            containedObjects.Clear();
        }
    }
}