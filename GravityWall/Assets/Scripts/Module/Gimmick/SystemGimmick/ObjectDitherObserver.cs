using System;
using System.Collections;
using System.Collections.Generic;
using Constants;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    public class ObjectDitherObserver : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float maxDistance;
        private readonly RaycastHit[] hitResults = new RaycastHit[16];
        private readonly Dictionary<GameObject, ObjectDither> hitObjects = new Dictionary<GameObject, ObjectDither>(16);
        private readonly HashSet<GameObject> containedObjects = new HashSet<GameObject>(16);
        private readonly List<GameObject> removeObjectsCache = new List<GameObject>(16);
        private const int layerMask = Layer.Mask.Base | Layer.Mask.IgnoreGravity;

        private void Update()
        {
            Ray ray = new Ray(transform.position, playerTransform.position - transform.position);

            int hitCount = Physics.RaycastNonAlloc(ray, hitResults, maxDistance, layerMask, QueryTriggerInteraction.Collide);

            for (int i = 0; i < hitCount; i++)
            {
                GameObject hitObj = hitResults[i].transform.gameObject;

                if (!hitObj.CompareTag(Tag.Dither))
                {
                    continue;
                }

                containedObjects.Add(hitObj);

                if (!hitObjects.ContainsKey(hitObj))
                {
                    if (hitObjects.Count == hitResults.Length)
                    {
                        continue;
                    }

                    var dither = hitObj.GetComponent<ObjectDither>();
                    hitObjects.Add(hitObj, dither);
                    dither.Show();
                }
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