using System;
using System.Collections.Generic;
using System.Linq;
using Module.Gimmick;
using UnityEngine;

namespace Application.Sequence
{
    /// <summary>
    /// レベルの復元を行うクラス
    /// </summary>
    [Serializable]
    public class LevelResetter 
    {
        [Header("リスポーン時に復元したいオブジェクト")]
        [SerializeField] private GameObject[] levelObjects;

        private readonly List<(GimmickObject gimmick, bool isEnable)> gimmicks = new List<(GimmickObject gimmick, bool enabled)>();
        private readonly List<Vector3> levelObjectPositions = new List<Vector3>();

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
                levelObjectPositions.Add(levelObject.transform.position);
            }
        }

        public void ResetLevel()
        {
            //座標の復元
            for (int i = 0; i < levelObjects.Length; i++)
            {
                levelObjects[i].gameObject.SetActive(true);
                levelObjects[i].transform.position = levelObjectPositions[i];
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