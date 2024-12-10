using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;
using Constants;
using Cysharp.Threading.Tasks;
using Module.Player;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Video;
namespace Module.Gimmick.SystemGimmick
{
    public class TutorialGuideSequencer : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera titleVirtualCamera;
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private GameObject movieCanvas;
        [SerializeField] private Transform pos;
        private PlayerController playerController;
        private bool isEnable = false;

        private void OnTriggerEnter(Collider other)
        {
            if (isEnable) return;

            if (other.gameObject.CompareTag(Tag.Player))
            {
                playerController = other.gameObject.GetComponent<PlayerController>();
                playerController.GetComponent<Rigidbody>().velocity = Vector3.zero;
                other.transform.position = pos.position;
                playerController.Lock();
                isEnable = true;

                TutorialGuidSequence().Forget();
            }
        }

        private async UniTaskVoid TutorialGuidSequence()
        {
            titleVirtualCamera.Priority = 100;

            await Task.Delay(2000);

            videoPlayer.Play();
            movieCanvas.SetActive(true);

            await Task.Delay(26000);

            titleVirtualCamera.Priority = 0;

            playerController.GetComponent<GravitySwitcher>().Enable();

            playerController.Unlock();
        }
    }
}