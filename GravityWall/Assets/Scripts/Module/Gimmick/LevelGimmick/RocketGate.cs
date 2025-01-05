using R3;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

namespace Module.Gimmick.LevelGimmick
{
    public class RocketGate : GimmickObject
    {
        [SerializeField] private GimmickObject[] observedSwitches;
        [SerializeField] UnityEvent openEvent;
        private Animator animator;

        private void Start()
        {
            animator = GetComponent<Animator>();

            foreach (GimmickObject gimmick in observedSwitches)
            {
                gimmick.IsEnabled.Skip(1).Subscribe(Enable).AddTo(this);
            }
        }

        public override void Disable(bool doEffect = true)
        {
            
        }

        public override void Enable(bool doEffect = true)
        {
            animator.SetBool("IsOpen",true);
        }

        public override void Reset()
        {
            
        }

        public void PlaySE()
        {

        }

        public void PlayEffect(int effectId)
        {
            openEvent.Invoke();
        }
    }
}