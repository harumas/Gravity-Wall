using UnityEngine;
using R3;
using UnityEngine.Serialization;

namespace Module.Character
{
    public class PlayerSoundPlayer : MonoBehaviour
    {
        [SerializeField] private float playInterval = 0.3f;
        [SerializeField] private AudioClip rotateClip;
        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private PlayerController playerController;
        private bool isRotating = true;

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

                if (this.isRotating == false)
                {
                    audioSource.volume = 0.8f;
                    audioSource.PlayOneShot(rotateClip);
                    lastPlayTime = Time.time;
                }

                this.isRotating = isRotating;
            }).AddTo(this);

            playerController.IsJumping.Subscribe(isJumping =>
            {
                if (isJumping)
                {
                    audioSource.volume = 0.5f;
                    audioSource.PlayOneShot(jumpClip);
                }
            }).AddTo(this);
        }
    }
}