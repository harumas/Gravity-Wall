using UnityEngine;

namespace Module.Player
{
    public class PlayerAnimationEventManager : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip footClip;
        [SerializeField] private Animator anim;
        private static readonly int speed = Animator.StringToHash("Speed");
        private static readonly int isJumping = Animator.StringToHash("IsJumping");

        public void FootEvent()
        {
            if (anim.GetFloat(speed) > 0.1f || anim.GetBool(isJumping))
            {
                audioSource.pitch = Random.Range(0.7f, 1.3f);
                audioSource.PlayOneShot(footClip);
            }
        }
    }
}