using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Module.Player;
using CoreModule.Sound;
using DG.Tweening;

namespace Module.Effect
{
    public class FallSEPlayer : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigidbody;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private float minVelocity;
        [SerializeField] private float minFallTime;
        [SerializeField] private AudioSource audioSource;

        private readonly float volumeChangeRate = 0.5f;
        private readonly float pitchChangeRate = 0.3f;
        private readonly float initialVolume = 0.3f;
        private readonly float initialPitch = 0.8f;
        private readonly float fadeTime = 0.3f;

        private Tween tween;

        private float timer;

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (!playerController.IsGrounding.CurrentValue && rigidbody.velocity.magnitude >= minVelocity)
            {
                timer += Time.deltaTime;

                if (timer < minFallTime) return;

                if (!audioSource.isPlaying) 
                {
                    audioSource.Play();
                }

                audioSource.volume += Time.deltaTime * volumeChangeRate;
                audioSource.pitch += Time.deltaTime * pitchChangeRate;
            }
            else
            {
                tween?.Kill();
                tween = DOTween.To(() => audioSource.volume,(v) => audioSource.volume = v,0, fadeTime).OnComplete(() =>
                {
                    audioSource.Pause();
                    audioSource.volume = initialVolume;
                    audioSource.pitch = initialPitch;
                    timer = 0;
                });
            }
        }
    }
}