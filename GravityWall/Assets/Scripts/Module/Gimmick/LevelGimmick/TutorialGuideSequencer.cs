using System;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Module.Gimmick.SystemGimmick;
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
        [SerializeField] private float delayTime = 2.6f;
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
            playerTrap.PlayPlayerInstallAnimation(true);

            await UniTask.Delay(TimeSpan.FromSeconds(videoPlayer.length / videoPlayer.playbackSpeed));

            titleVirtualCamera.Priority = 0;

            playerTrap.Disable(true);
            playerTrap.PlayPlayerInstallAnimation(false);
        }
    }
}