using System;
using Cinemachine;
using CoreModule.Sound;
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
        [SerializeField] private CameraShaker cameraShaker;
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private GameObject movieCanvas,huckingEffect;
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

            await UniTask.Delay(TimeSpan.FromSeconds(4));
            playerTrap.PlayPlayerInstallAnimation(true);

            await UniTask.Delay(TimeSpan.FromSeconds(2));

            SoundManager.Instance.Play(Core.Sound.SoundKey.ElectricShock, Core.Sound.MixerType.SE);
            huckingEffect.SetActive(true);
            cameraShaker.ShakeCamera(10,0.5f);

            await UniTask.Delay(TimeSpan.FromSeconds(2));
            cameraShaker.ShakeCamera(0, 0);
            huckingEffect.SetActive(false);

            await UniTask.Delay(TimeSpan.FromSeconds(1));

            playerTrap.PlayPlayerInstallAnimation(false);


            await UniTask.Delay(TimeSpan.FromSeconds(videoPlayer.length / videoPlayer.playbackSpeed - 16));

            titleVirtualCamera.Priority = 0;

            playerTrap.Disable(true);
        }
    }
}