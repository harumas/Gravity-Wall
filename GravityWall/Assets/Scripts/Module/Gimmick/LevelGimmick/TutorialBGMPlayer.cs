using UnityEngine;
using Module.Gravity;
using Constants;
using DG.Tweening;

namespace Module.LevelGimmick
{
    public class TutorialBGMPlayer : MonoBehaviour
    {
        [SerializeField] private float fadeTime = 0.3f;
        [SerializeField] private Direction direction;
        enum Direction
        {
            none,
            back,
            forward,
            right,
            left
        }

        private AudioSource audioSource;
        bool isPlaying = false;
        private Tween tween;
        private float volume;
        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            volume = audioSource.volume;
        }

        private void OnTriggerStay(Collider other)
        {
            if (isPlaying) return;

            if (other.CompareTag(Tag.Player))
            {
                if (WorldGravity.Instance.Gravity == GetDirection() || direction == Direction.none)
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
            if (!isPlaying) return;
            if (other.CompareTag(Tag.Player))
            {
                tween?.Kill();
                tween = DOTween.To(() => audioSource.volume,(v) => audioSource.volume = v,0, fadeTime).OnComplete(() =>
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
                case Direction.back:
                    return Vector3.back;
                case Direction.forward:
                    return Vector3.forward;
            }

            return Vector3.forward;
        }
    }
}