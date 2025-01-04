using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Gimmick.LevelGimmick
{
    public class EndingHologramSetter : MonoBehaviour
    {
        [SerializeField] private GameObject colonyHoloObject, namePlate;

        public void SetHologram()
        {
            namePlate.SetActive(false);
            colonyHoloObject.SetActive(true);
            Debug.Log("");
        }
    }
}
