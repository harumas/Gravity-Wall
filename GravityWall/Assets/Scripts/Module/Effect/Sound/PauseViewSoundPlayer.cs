using Core.Sound;
using CoreModule.Sound;
using R3;
using UnityEngine;
using View;

namespace Module.Effect.Sound
{
    /// <summary>
    /// ポーズ画面のサウンドを再生するクラス
    /// </summary>
    public class PauseViewSoundPlayer : MonoBehaviour
    {
        [SerializeField] private PauseView pauseView;

        private void Awake()
        {
            pauseView.OnContinueButtonSelected.Subscribe(_ => PlaySelect()).AddTo(this);
            pauseView.OnContinueButtonPressed.Subscribe(_ => PlaySubmit()).AddTo(this);
            
            pauseView.OnRestartButtonSelected.Subscribe(_ => PlaySelect()).AddTo(this);
            pauseView.OnRestartButtonPressed.Subscribe(_ => PlaySubmit()).AddTo(this);
            
            pauseView.OnReturnToHubButtonSelected.Subscribe(_ => PlaySelect()).AddTo(this);
            pauseView.OnReturnToHubButtonPressed.Subscribe(_ => PlaySubmit()).AddTo(this);
            
            pauseView.OnEndGameButtonSelected.Subscribe(_ => PlaySelect()).AddTo(this);
            pauseView.OnEndGameButtonPressed.Subscribe(_ => PlaySubmit()).AddTo(this);
            
            pauseView.OnGoToSettingsButtonSelected.Subscribe(_ => PlaySelect()).AddTo(this);
            pauseView.OnGoToSettingsButtonPressed.Subscribe(_ => PlaySubmit()).AddTo(this);
        }

        private void PlaySelect()
        {
            if (pauseView.IsFirstSelect)
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