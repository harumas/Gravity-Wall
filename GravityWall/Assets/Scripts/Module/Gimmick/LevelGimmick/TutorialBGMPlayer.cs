using UnityEngine;
using Module.Gravity;
using Constants;

namespace Module.LevelGimmick
{
    public class TutorialBGMPlayer : MonoBehaviour
    {
        private AudioSource audioSource;
        bool isPlaying = false;
        [SerializeField] private Direction direction;
        enum Direction
        {
            none,
            back,
            forward,
            right,
            left
        }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerStay(Collider other)
        {
            if (isPlaying) return;

            if (other.CompareTag(Tag.Player))
            {
                if (WorldGravity.Instance.Gravity == GetDirection() || direction == Direction.none)
                {
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
                audioSource.Stop();
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