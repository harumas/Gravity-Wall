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

        private const int MaxBufferCount = 16;
        private const int LayerMask = Layer.Mask.Base | Layer.Mask.IgnoreGravity;

        private readonly RaycastHit[] hitResults = new RaycastHit[MaxBufferCount];
        private readonly Dictionary<GameObject, DitherObject> hitObjects = new(MaxBufferCount);
        private readonly HashSet<GameObject> containedObjects = new(MaxBufferCount);
        private readonly Queue<GameObject> removeObjectsQueue = new(MaxBufferCount);

        private void Update()
        {
            if (hitObjects.Count < MaxBufferCount)
            {
                // カメラからプレイヤーまでのオブジェクトを取得
                Ray ray = new Ray(transform.position, playerTransform.position - transform.position);
                int hitCount = Physics.RaycastNonAlloc(ray, hitResults, maxDistance, LayerMask, QueryTriggerInteraction.Collide);

                DitherHitObjects(hitCount);
            }

            foreach ((GameObject obj, DitherObject dither) in hitObjects)
            {
                // レイキャストにヒットしなかった場合は透過処理を解除して削除キューに入れる
                if (!containedObjects.Contains(obj))
                {
                    dither.Show();
                    removeObjectsQueue.Enqueue(obj);
                }
            }

            containedObjects.Clear();

            // レイキャストから外れたオブジェクトを削除
            while (removeObjectsQueue.Count > 0)
            {
                hitObjects.Remove(removeObjectsQueue.Dequeue());
            }
        }

        private void DitherHitObjects(int hitCount)
        {
            for (int i = 0; i < hitCount; i++)
            {
                GameObject hitObj = hitResults[i].transform.gameObject;

                if (!hitObj.CompareTag(Tag.Dither))
                {
                    continue;
                }

                containedObjects.Add(hitObj);

                // 既に透過処理がされている場合はスキップ
                if (hitObjects.ContainsKey(hitObj))
                {
                    continue;
                }

                var dither = hitObj.GetComponent<DitherObject>();
                hitObjects.Add(hitObj, dither);
                dither.Dither();
            }
        }
    }
}