using System;
using Core.Sound;
using CoreModule.Sound;
using R3;
using UnityEngine;
using View;

namespace Module.Effect.Sound
{
    public class OptionSoundPlayer : MonoBehaviour
    {
        [SerializeField] private OptionView optionView;

        private void Awake()
        {
            optionView.OnBackButtonPressed.Subscribe(_ => PlaySubmit()).AddTo(this);
            optionView.OnBackButtonSelected.Subscribe(_ => PlaySelect()).AddTo(this);
            
            optionView.OnLicenseButtonPressed.Subscribe(_ => PlaySubmit()).AddTo(this);
            optionView.OnLicenseButtonSelected.Subscribe(_ => PlaySelect()).AddTo(this);
            
            optionView.OnBgmVolumeChanged.Skip(2).Subscribe(_ => PlaySelect()).AddTo(this);
            optionView.OnSeVolumeChanged.Skip(2).Subscribe(_ => PlaySelect()).AddTo(this);
            optionView.OnControllerSensibilityChanged.Skip(2).Subscribe(_ => PlaySelect()).AddTo(this);
            optionView.OnVibrationToggleChanged.Skip(2).Subscribe(_ => PlaySelect()).AddTo(this);
        }

        private void PlaySelect()
        {
            if (optionView.IsFirstSelect) 
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