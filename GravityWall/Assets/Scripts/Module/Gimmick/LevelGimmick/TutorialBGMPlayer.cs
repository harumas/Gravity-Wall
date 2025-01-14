using Constants;
using DG.Tweening;
using Module.Gravity;
using UnityEngine;

namespace Module.Gimmick.LevelGimmick
{
    public class TutorialBGMPlayer : MonoBehaviour
    {
        [SerializeField] private float fadeTime = 0.3f;
        [SerializeField] private Direction direction;

        private enum Direction
        {
            None,
            Back,
            Forward,
            Right,
            Left
        }

        private AudioSource audioSource;
        private bool isPlaying = false;
        private Tween tween;
        private float volume;
        
        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            volume = audioSource.volume;
        }

        private void OnTriggerStay(Collider other)
        {
            if (isPlaying)
            {
                return;
            }

            if (other.CompareTag(Tag.Player))
            {
                if (WorldGravity.Instance.Gravity == GetDirection() || direction == Direction.None)
                {
                    tween?.Kill();
                    audioSource.volume = volume;
                    audioSource.Play();
                    isPlaying = true;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!isPlaying)
            {
                return;
            }
            
            if (other.CompareTag(Tag.Player))
            {
                tween?.Kill();
                tween = DOTween.To(() => audioSource.volume, (v) => audioSource.volume = v, 0, fadeTime).OnComplete(() =>
                {
                    audioSource.Stop();
                });
                isPlaying = false;
            }
        }

        private Vector3 GetDirection()
        {
            switch (direction)
            {
                case Direction.Back:
                    return Vector3.back;
                case Direction.Forward:
                    return Vector3.forward;
            }

            return Vector3.forward;
        }
    }
}