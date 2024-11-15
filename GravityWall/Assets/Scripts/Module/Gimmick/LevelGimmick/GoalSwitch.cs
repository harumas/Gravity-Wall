using System.Collections;
using System.Collections.Generic;
using Constants;
using Module.Character;
using UnityEngine;
using UnityEngine.Events;

namespace Module.Gimmick
{
    public class GoalSwitch : GimmickObject
    {
        [SerializeField] private Animator animator;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Transform SwitchPos;
        [SerializeField] private PowerPipe[] powerPipes;
        [SerializeField] private UnityEvent goalEvent;

        public override void Disable(bool doEffect = true)
        {

        }

        public override void Enable(bool doEffect = true)
        {

        }

        public override void Reset()
        {

        }

        private PlayerController playerController;
        private void OnTriggerEnter(Collider other)
        {
            if (isEnabled.Value) return;

            if (other.gameObject.CompareTag(Tag.Player))
            {
                playerController = other.gameObject.GetComponent<PlayerController>();
                playerController.GetComponent<Rigidbody>().velocity = Vector3.zero;
                playerController.transform.position = SwitchPos.position;
                playerController.Lock();
                animator.SetTrigger("OnSwitch");
            }
        }

        public void OnSEPlay(float count)
        {
            audioSource.pitch = count;
            audioSource.Play();
        }

        public void OnEvent()
        {
            if (playerController == null) return;

            playerController.Unlock();

            foreach (var pipe in powerPipes)
            {
                pipe.OnPowerPipe(true);
            }

            isEnabled.Value = true;

            //カットシーン
            goalEvent.Invoke();
        }
    }
}