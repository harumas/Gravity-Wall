using Constants;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    public class StartRoomShowTrigger : MonoBehaviour
    {
        [SerializeField] private StartRoom startRoom;
        [SerializeField] private GameObject roomBody;
        [SerializeField] private GameObject levelPipe;

        private bool isPlayerEnter = false;

        private void Start()
        {
            roomBody.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                roomBody.SetActive(true);
                isPlayerEnter = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!startRoom.IsPlayerEnter && other.CompareTag(Tag.Player))
            {
                roomBody.SetActive(false);
                isPlayerEnter = false;
            }
        }

        public void Reset()
        {
            roomBody.SetActive(false);
            levelPipe.SetActive(true);
            isPlayerEnter = false;
        }

        private void OnDisable()
        {
            if (!isPlayerEnter)
            {
                levelPipe.SetActive(false);
            }
        }
    }
}