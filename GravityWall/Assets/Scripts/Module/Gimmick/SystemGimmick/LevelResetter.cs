using System;
using System.Collections.Generic;
using System.Linq;
using Module.Gimmick.LevelGimmick;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    internal struct TransformData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Rigidbody Rigidbody;

        public TransformData(Vector3 position, Quaternion rotation, Rigidbody rigidbody)
        {
            Position = position;
            Rotation = rotation;
            Rigidbody = rigidbody;
        }
    }

    /// <summary>
    /// レベルの復元を行うクラス
    /// </summary>
    [Serializable]
    public class LevelResetter
    {
        [Header("リスポーン時に復元したいオブジェクト")] [SerializeField] private GameObject[] levelObjects;

        private readonly List<(GimmickObject gimmick, bool isEnable)> gimmicks = new List<(GimmickObject gimmick, bool enabled)>();
        private readonly List<TransformData> levelObjectTransforms = new List<TransformData>();

        public void RegisterObjects()
        {
            //オブジェクトがギミックの場合は取得
            foreach (GameObject levelObject in levelObjects)
            {
                if (levelObject.TryGetComponent(out GimmickObject gimmick))
                {
                    gimmicks.Add((gimmick, gimmick.IsEnabled.CurrentValue));
                }
            }

            foreach (var levelObject in levelObjects)
            {
                Rigidbody rig = levelObject.GetComponent<Rigidbody>();
                var transformData = new TransformData(levelObject.transform.position, levelObject.transform.rotation, rig);
                levelObjectTransforms.Add(transformData);
            }
        }

        public void ResetLevel()
        {
            //座標の復元
            for (int i = 0; i < levelObjects.Length; i++)
            {
                levelObjects[i].gameObject.SetActive(true);
                TransformData transformData = levelObjectTransforms[i];

                if (transformData.Rigidbody != null)
                {
                    transformData.Rigidbody.velocity = Vector3.zero;
                    transformData.Rigidbody.angularVelocity = Vector3.zero;
                }

                levelObjects[i].transform.SetPositionAndRotation(transformData.Position, transformData.Rotation);
            }

            //ギミックはリセット
            foreach (GimmickObject gimmick in gimmicks.Select(item => item.gimmick))
            {
                gimmick.Reset();
            }

            //ギミックの有効状態を復元
            foreach (var (gimmick, isEnable) in gimmicks)
            {
                if (isEnable)
                {
                    gimmick.Enable(false);
                }
                else
                {
                    gimmick.Disable(false);
                }
            }
        }
    }
}