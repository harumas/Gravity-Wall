using Cinemachine;
using Constants;
using UnityEngine;

namespace Module.Gimmick
{
    public class CameraChanger : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(Tag.Player))
            {
                cinemachineVirtualCamera.Priority = 11;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag(Tag.Player))
            {
                cinemachineVirtualCamera.Priority = 9;
            }
        }
    }
}