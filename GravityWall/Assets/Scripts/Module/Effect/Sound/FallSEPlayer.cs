using Cysharp.Threading.Tasks;
using DG.Tweening;
using Module.Gravity;
using Module.Player;
using UnityEngine;

namespace Module.Effect.Sound
{
    public class FallSEPlayer : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigidbody;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private float minVelocity;
        [SerializeField] private float minFallTime;
        [SerializeField] private float fadeInDuration;
        [SerializeField] private float fadeInPitchDuration;
        [SerializeField] private float fadeOutDuration;
        [SerializeField] private float initialVolume;
        [SerializeField] private float initialPitch;
        [SerializeField] private AudioSource audioSource;

        private Tween volumeTween;
        private Tween pitchTween;
        private Tween fadeOutTween;
        private float timer;

        private void Update()
        {
            Vector3 velocity = rigidbody.velocity;
            Vector3 gravity = WorldGravity.Instance.Gravity;
            
            // 重力方向に一定速度以上で落下しているか
            bool isFalling = !playerController.IsGrounding.CurrentValue && 
                             Vector3.Dot(velocity, gravity) > 0 &&
                             velocity.sqrMagnitude >= minVelocity * minVelocity;

            if (isFalling)
            {
                timer += Time.deltaTime;

                if (timer < minFallTime)
                {
                    return;
                }

                FadeIn();
            }
            else
            {
                FadeOut();
            }
        }

        private void FadeIn()
        {
            if (audioSource.isPlaying)
            {
                return;
            }

            fadeOutTween?.Kill();
            Reset();
            audioSource.Play();

            volumeTween = DOTween.To(() => audioSource.volume, (v) => audioSource.volume = v, 1, fadeInDuration);
            pitchTween = DOTween.To(() => audioSource.pitch, (v) => audioSource.pitch = v, 3, fadeInPitchDuration);
        }

        private void FadeOut()
        {
            if (!audioSource.isPlaying)
            {
                return;
            }

            volumeTween?.Kill();
            pitchTween?.Kill();

            // 音量をフェードアウト
            fadeOutTween = DOTween.To(() => audioSource.volume, (v) => audioSource.volume = v, 0, fadeOutDuration).OnComplete(Reset);
        }

        private void Reset()
        {
            // 再生を停止して初期化
            audioSource.Pause();
            audioSource.volume = initialVolume;
            audioSource.pitch = initialPitch;
            timer = 0;
        }
    }
}