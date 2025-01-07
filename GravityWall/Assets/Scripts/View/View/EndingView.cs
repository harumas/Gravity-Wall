using R3;
using R3.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

namespace View
{
    public class EndingView : MonoBehaviour
    {
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Button continueButton,newGameButton;

        public CanvasGroup CanvasGroup => canvasGroup;
        public VideoPlayer VideoPlayer => videoPlayer;

        public bool IsFirstSelect { get; private set; }

        ReactiveProperty<bool> isVideoPlaying = new ReactiveProperty<bool>(false);
        public ReadOnlyReactiveProperty<bool> IsVideoPlaying => isVideoPlaying;

        public Observable<Unit> OnContinueButtonPressed => continueButton.OnClickAsObservable();
        public Observable<Unit> OnNewGameButtonPressed => newGameButton.OnClickAsObservable();

        public Observable<BaseEventData> OnContinueButtonSelected => continueButton.OnSelectAsObservable();
        public Observable<BaseEventData> OnNewGameButtonSelected => newGameButton.OnSelectAsObservable();

        public void SelectFirst()
        {
            IsFirstSelect = true;
            EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
            IsFirstSelect = false;
        }
    }
}