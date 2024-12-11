using PropertyGenerator.Generated;
using UnityEngine;

namespace Module.Player
{
    public class PlayerAnimationEventManager : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip footClip;
        [SerializeField] private float speedThreshold;
        
        [SerializeField] private PlayerControllerWrapper anim;

        public void FootEvent()
        {
            if (anim.Speed > speedThreshold || anim.IsJumping)
            {
                audioSource.volume = 0.3f;
                audioSource.pitch = Random.Range(0.7f, 1.3f);
                audioSource.PlayOneShot(footClip);
            }
        }
    }
}