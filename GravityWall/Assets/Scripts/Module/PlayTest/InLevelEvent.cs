using System.Collections;
using System.Collections.Generic;
using Constants;
using Module.Gimmick;
using UnityEngine;

namespace Module.PlayTest
{
    public class InLevelEvent : MonoBehaviour
    {
        [SerializeField] private GameObject[] hallways;
        [SerializeField] private Gate gate;
        [SerializeField] private LevelActiveChanger levelActiveChanger;
        private bool inRoomPlayer;

        private ObjectHider objectHider;
        void Start()
        {
            if (gate.GetComponent<ObjectHider>() != null)
            {
                objectHider = gate.GetComponent<ObjectHider>();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.CompareTag(Tag.Player) && !gate.IsEnabled.CurrentValue && !inRoomPlayer)
            {
                objectHider?.Enable();
                levelActiveChanger.SetActiveLevel(hallways);
                inRoomPlayer = true;
            }
        }
    }
}