using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Module.PlayTest
{
    public class LevelActiveChanger : MonoBehaviour
    {
        [SerializeField] private GameObject[] levelObjects;

        public void SetActiveLevel(GameObject[] objects)
        {
            foreach (var levelObject in levelObjects)
            {
                levelObject.SetActive(objects.Contains(levelObject));
            }
        }
    }
}