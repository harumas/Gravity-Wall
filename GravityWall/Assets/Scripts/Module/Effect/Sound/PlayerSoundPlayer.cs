using System.Media;
using Core.Sound;
using CoreModule.Sound;
using Module.Player;
using R3;
using UnityEngine;

namespace Module.Effect.Sound
{
    public class PlayerSoundPlayer : MonoBehaviour
    {
        [SerializeField] private float playInterval = 0.3f;
        [SerializeField] private PlayerController playerController;

        private float lastPlayTime;

        void Start()
        {
            // 回転のイベント登録
            playerController.IsRotating.Subscribe(isRotating =>
                {
                    if (lastPlayTime + playInterval > Time.time)
                    {
                        return;
                    }

                    if (isRotating)
                    {
                        SoundManager.Instance.Play(SoundKey.WallRotate, MixerType.SE);
                        lastPlayTime = Time.time;
                    }
                })
                .AddTo(this);

            playerController.IsJumping.Subscribe(isJumping =>
                {
                    if (isJumping)
                    {
                        SoundManager.Instance.Play(SoundKey.Jump, MixerType.SE);
                    }
                })
                .AddTo(this);
        }
    }
}