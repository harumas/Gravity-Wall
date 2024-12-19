using System;
using System.Threading.Tasks;
using Cinemachine;
using Constants;
using Cysharp.Threading.Tasks;
using Module.Gimmick.SystemGimmick;
using Module.Player;
using UnityEngine;
using UnityEngine.Video;

namespace Module.Gimmick.LevelGimmick
{
    public class TutorialGuideSequencer : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera titleVirtualCamera;
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private GameObject movieCanvas;
        [SerializeField] private InGameEventPlayerTrap playerTrap;
        [SerializeField] private float delayTime = 26;
        [SerializeField] private float videoStartDelayTime = 2;
        
        private void Start()
        {
            playerTrap.OnTrapped += () => TutorialGuidSequence().Forget();
        }

        private async UniTaskVoid TutorialGuidSequence()
        {
            titleVirtualCamera.Priority = 100;

            await UniTask.Delay(TimeSpan.FromSeconds(videoStartDelayTime));

            videoPlayer.Play();
            movieCanvas.SetActive(true);
            
            await UniTask.Delay(TimeSpan.FromSeconds(delayTime));

            titleVirtualCamera.Priority = 0;

            playerTrap.Disable();
        }
    }
}