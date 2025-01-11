using System.Collections;
using System.Collections.Generic;
using CoreModule.Save;
using UnityEngine;
using View;
using R3;
using DG.Tweening;
using Module.Config;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using VContainer;
using VContainer.Unity;

namespace Presentation
{
    public class EndingPresenter : IStartable
    {
        private readonly EndingView endingView;
        private readonly SaveManager<SaveData> saveManager;
        private bool isSelected = false;

        [Inject]
        public EndingPresenter(EndingView endingView, SaveManager<SaveData> saveManager)
        {
            this.endingView = endingView;
            this.saveManager = saveManager;
        }

        public void Start()
        {
            endingView.OnContinueButtonPressed.Subscribe(_ => OnPressedButtonEvent("Hub"));
            endingView.OnNewGameButtonPressed.Subscribe(_ => OnPressedButtonEvent("Hub-Additive"));

            //‰¹—ÊÝ’è
            //endingView.VideoPlayer.SetDirectAudioVolume(0,1);
            endingView.VideoPlayer.loopPointReached += OnVideoEnd;
            endingView.VideoPlayer.Play();
        }

        private void OnVideoEnd(VideoPlayer vp)
        {
            endingView.VideoPlayer.gameObject.SetActive(false);
            endingView.gameObject.SetActive(true);
            endingView.SelectFirst();
            DOTween.To(() => endingView.CanvasGroup.alpha, (alpha) => endingView.CanvasGroup.alpha = alpha, 1, 1.0f).SetDelay(1);
        }

        private void OnPressedButtonEvent(string sceneName)
        {
            if (isSelected) return;

            isSelected = true;

            DOTween.To(() => endingView.CanvasGroup.alpha, (alpha) => endingView.CanvasGroup.alpha = alpha, 0, 1.0f).OnComplete(() =>
            {
                saveManager.Reset();
                SceneManager.LoadScene(sceneName);
            });
        }
    }
}