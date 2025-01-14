using Core.Sound;
using CoreModule.Sound;
using R3;
using UnityEngine;
using View.View;

namespace Module.Effect.Sound
{
    /// <summary>
    /// ゲーム初期化の確認画面のサウンドを再生するクラス
    /// </summary>
    public class ConfirmNewGameViewSoundPlayer : MonoBehaviour
    {
        [SerializeField] private ConfirmNewGameView confirmNewGameView;
        
        private void Awake()
        {
            confirmNewGameView.OnCancelButtonPressed.Subscribe(_ => PlaySubmit()).AddTo(this);
            confirmNewGameView.OnCancelButtonPressed.Subscribe(_ => PlaySelect()).AddTo(this);

            confirmNewGameView.OnConfirmButtonPressed.Subscribe(_ => PlaySubmit()).AddTo(this);
            confirmNewGameView.OnConfirmButtonPressed.Subscribe(_ => PlaySelect()).AddTo(this);
        }

        private void PlaySelect()
        {
            if (confirmNewGameView.IsFirstSelect)
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