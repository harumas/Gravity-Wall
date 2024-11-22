using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Gimmick
{
    public class HubGate : GimmickObject
    {
        [SerializeField] private AudioClip openAudioClip, unlockAudioClip;
        private Animator animator;
        private AudioSource audioSource;
        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
        }

        public void OpenEvent()
        {
            animator.SetTrigger("IsOpen");
        }

        public void OpenAnimationEvent()
        {
            audioSource.PlayOneShot(openAudioClip);
        }

        public void UnlockAnimationEvent()
        {
            audioSource.PlayOneShot(unlockAudioClip);
        }

        public override void Enable(bool doEffect = true)
        {

        }

        public override void Disable(bool doEffect = true)
        {

        }

        public override void Reset()
        {

        }
    }
}