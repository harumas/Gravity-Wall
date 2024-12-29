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

                audioSource.volume += Time.deltaTime * 0.1f;
                audioSource.pitch += Time.deltaTime * 0.1f;
            }
            else
            {
                tween?.Kill();
                tween = DOTween.To(() => audioSource.volume,(v) => audioSource.volume = v,0,0.3f).OnComplete(() =>
                {
                    audioSource.Pause();
                    audioSource.volume = 0.3f;
                    audioSource.pitch = 0.8f;
                    timer = 0;
                });
            }
        }
    }
}