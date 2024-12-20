using System;
using Core.Sound;
using CoreModule.Sound;
using R3;
using UnityEngine;
using View;

namespace Module.Effect.Sound
{
    public class TitleSoundPlayer : MonoBehaviour
    {
        [SerializeField] private TitleView titleView;

        private void Awake()
        {
            titleView.OnNewGameButtonSelected.Subscribe(_ => PlaySelect()).AddTo(this);
            titleView.OnNewGameButtonPressed.Subscribe(_ => PlaySubmit()).AddTo(this);
            
            titleView.OnCreditButtonSelected.Subscribe(_ => PlaySelect()).AddTo(this);
            titleView.OnCreditButtonPressed.Subscribe(_ => PlaySubmit()).AddTo(this);
            
            titleView.OnContinueGameButtonSelected.Subscribe(_ => PlaySelect()).AddTo(this);
            titleView.OnContinueGameButtonPressed.Subscribe(_ => PlaySubmit()).AddTo(this);
            
            titleView.OnEndGameButtonSelected.Subscribe(_ => PlaySelect()).AddTo(this);
            titleView.OnEndGameButtonPressed.Subscribe(_ => PlaySubmit()).AddTo(this);
        }

        private void PlaySelect()
        {
            SoundManager.Instance.Play(SoundKey.ButtonSelect, MixerType.SE);
        }
        
        private void PlaySubmit()
        {
            SoundManager.Instance.Play(SoundKey.ButtonSubmit, MixerType.SE);
        }
    }
}