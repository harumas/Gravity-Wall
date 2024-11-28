using R3;
using UnityEngine;

namespace Module.Player
{
    public class PlayerSoundPlayer : MonoBehaviour
    {
        [SerializeField] private float playInterval = 0.3f;
        [SerializeField] private AudioClip rotateClip;
        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private AudioSource audioSource;
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
                        audioSource.PlayOneShot(rotateClip);
                        lastPlayTime = Time.time;
                    }
                })
                .AddTo(this);

            playerController.IsJumping.Subscribe(isJumping =>
                {
                    if (isJumping)
                    {
                        audioSource.PlayOneShot(jumpClip);
                    }
                })
                .AddTo(this);
        }
    }
}