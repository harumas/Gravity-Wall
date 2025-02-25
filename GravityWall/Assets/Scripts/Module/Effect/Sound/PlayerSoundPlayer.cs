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
            playerController.ControlEvent.IsRotating.Subscribe(isRotating =>
                {
                    // 再生にクールダウンを設ける
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

            // ジャンプのイベント登録
            playerController.ControlEvent.IsExternalForce.Subscribe(isJumping =>
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