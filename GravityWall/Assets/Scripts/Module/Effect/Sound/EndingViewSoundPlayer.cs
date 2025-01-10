using Core.Sound;
using CoreModule.Sound;
using R3;
using UnityEngine;
using View;

namespace Module.Effect.Sound
{
    public class EndingViewSoundPlayer : MonoBehaviour
    {
        [SerializeField] private EndingView endingView;

        private void Start()
        {
            endingView.OnContinueButtonPressed.Subscribe(_ => PlaySubmit()).AddTo(this);
            endingView.OnContinueButtonSelected.Subscribe(_ => PlaySelect()).AddTo(this);
            endingView.OnNewGameButtonPressed.Subscribe(_ => PlaySubmit()).AddTo(this);
            endingView.OnNewGameButtonSelected.Subscribe(_ => PlaySelect()).AddTo(this);
        }

        private void PlaySelect()
        {
            if (endingView.IsFirstSelect)
            {
                return;
            }

            SoundManager.Instance.Play(SoundKey.ButtonSelect, MixerType.SE);
        }

        private void PlaySubmit()
        {
            SoundManager.Instance.Play(SoundKey.ButtonSubmit, MixerType.SE);
        }
    }
}