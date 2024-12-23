using System;
using Constants;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    public class StartRoomShowTrigger : MonoBehaviour
    {
        [SerializeField] private StartRoom startRoom;
        [SerializeField] private GameObject roomBody;

        private void Start()
        {
            roomBody.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                roomBody.SetActive(true);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!startRoom.IsPlayerEnter && other.CompareTag(Tag.Player))
            {
                roomBody.SetActive(false);
            }
        }
    }
}