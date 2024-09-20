using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Module.Gimmick;
using UnityEngine;

namespace Module.PlayTest
{
    public class LevelReseter : MonoBehaviour
    {
        [SerializeField] private GameObject[] levelObjects;
        [SerializeField] private AbstractGimmickAffected[] gimmicks;
        private List<Vector3> levelObjectPositions = new List<Vector3>();
        // Start is called before the first frame update
        void Start()
        {
            foreach (var levelObject in levelObjects)
            {
                levelObjectPositions.Add(levelObject.transform.position);
            }
        }

        public void ResetLevel()
        {
            for (int i = 0; i < levelObjects.Length; i++)
            {
                levelObjects[i].transform.position = levelObjectPositions[i];
            }

            foreach (var gimmick in gimmicks)
            {
                gimmick.Reset();
            }
        }
    }
}