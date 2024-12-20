using Module.Gimmick.SystemGimmick;
using PropertyGenerator.Generated;
using UnityEngine;
using UnityEngine.Events;

namespace Module.Gimmick.LevelGimmick
{
    public class GoalSwitch : GimmickObject
    {
        [SerializeField] private ImportantSwitchWrapper animator;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private PowerPipe[] powerPipes;
        [SerializeField] private UnityEvent goalEvent;
        [SerializeField] private InGameEventPlayerTrap playerTrap;

        private void Start()
        {
            playerTrap.OnTrapped += () =>
            {
                animator.SetOnSwitchTrigger();
            };
        }

        public override void Disable(bool doEffect = true)
        {

        }

        public override void Enable(bool doEffect = true)
        {

        }

        public override void Reset()
        {

        }

        public void OnSEPlay(float count)
        {
            audioSource.pitch = count;
            audioSource.Play();
        }

        public void OnEvent()
        {
            playerTrap.Disable();

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