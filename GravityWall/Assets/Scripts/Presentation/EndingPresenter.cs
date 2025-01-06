using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using View;
using R3;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace Presentation
{
    public class EndingPresenter : MonoBehaviour
    {
        [SerializeField] EndingView endingView;
        private bool isSelected = false;
        // Start is called before the first frame update
        void Start()
        {
            endingView.OnContinueButtonPressed.Subscribe(_ => OnPressedButtonEvent("Hub"));
            endingView.OnNewGameButtonPressed.Subscribe(_ => OnPressedButtonEvent("Hub-Additive"));


            //‰¹—ÊÝ’è
            //endingView.VideoPlayer.SetDirectAudioVolume(0,1);
            endingView.VideoPlayer.loopPointReached += OnVideoEnd;
            endingView.VideoPlayer.Play();

            endingView.SelectFirst();
        }

        void OnVideoEnd(VideoPlayer vp)
        {
            endingView.VideoPlayer.gameObject.SetActive(false);
            endingView.gameObject.SetActive(true);
            DOTween.To(() => endingView.CanvasGroup.alpha, (alpha) => endingView.CanvasGroup.alpha = alpha, 1, 1.0f).SetDelay(1);
        }

        void OnPressedButtonEvent(string sceneName)
        {
            if (isSelected) return;

            isSelected = true;

            DOTween.To(() => endingView.CanvasGroup.alpha, (alpha) => endingView.CanvasGroup.alpha = alpha, 0, 1.0f).OnComplete(() =>
            {
                SceneManager.LoadScene(sceneName);
            });
        }
    }
}