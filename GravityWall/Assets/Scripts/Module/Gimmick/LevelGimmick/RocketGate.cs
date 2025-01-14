using R3;
using UnityEngine;
using UnityEngine.Events;

namespace Module.Gimmick.LevelGimmick
{
    public class RocketGate : GimmickObject
    {
        [SerializeField] private GimmickObject[] observedSwitches;
        [SerializeField] private UnityEvent openEvent;
        private Animator animator;
        private static readonly int isOpen = Animator.StringToHash("IsOpen");

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
            animator.SetBool(isOpen,true);
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