using Core.Sound;
using CoreModule.Sound;
using R3;
using UnityEngine;
using View.View;

namespace Module.Effect.Sound
{
    /// <summary>
    /// クレジット画面のサウンドを再生するクラス
    /// </summary>
    public class CreditViewSoundPlayer : MonoBehaviour
    {
        [SerializeField] private CreditView creditView;
        
        private void Awake()
        {
            creditView.OnBackButtonPressed.Subscribe(_ => PlaySubmit()).AddTo(this);
            creditView.OnBackButtonSelected.Subscribe(_ => PlaySelect()).AddTo(this);
        }

        private void PlaySelect()
        {
            if (creditView.IsFirstSelect)
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