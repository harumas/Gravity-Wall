using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Character
{
    public class PlayerAnimationEventManager : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip footClip;
        [SerializeField] private Animator anim;

        public void FootSE()
        {
            if (anim.GetFloat("Speed") > 0.1f || anim.GetBool("IsJumping"))
            {
                audioSource.volume = 0.3f;
                audioSource.pitch = Random.Range(0.7f, 1.3f);
                audioSource.PlayOneShot(footClip);
            }
        }
    }
}