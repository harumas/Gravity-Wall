using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace View
{
    public class PauseView : MonoBehaviour
    {
        [SerializeField] private Button continueButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button returnToHubButton;
        [SerializeField] private Button goToSettingsButton;
        [SerializeField] private Button endGameButton;
        [SerializeField] private CanvasGroup canvasGroup;
        
        public CanvasGroup CanvasGroup => canvasGroup;

        public Observable<Unit> OnContinueButtonPressed => continueButton.OnClickAsObservable();
        public Observable<Unit> OnRestartButtonPressed => restartButton.OnClickAsObservable();
        public Observable<Unit> OnReturnToHubButtonPressed => returnToHubButton.OnClickAsObservable();
        public Observable<Unit> OnGoToSettingsButtonPressed => goToSettingsButton.OnClickAsObservable();
        public Observable<Unit> OnEndGameButtonPressed => endGameButton.OnClickAsObservable();
        
        public Observable<BaseEventData> OnContinueButtonSelected => continueButton.OnSelectAsObservable();
        public Observable<BaseEventData> OnRestartButtonSelected => restartButton.OnSelectAsObservable();
        public Observable<BaseEventData> OnReturnToHubButtonSelected => returnToHubButton.OnSelectAsObservable();
        public Observable<BaseEventData> OnGoToSettingsButtonSelected => goToSettingsButton.OnSelectAsObservable();
        public Observable<BaseEventData> OnEndGameButtonSelected => endGameButton.OnSelectAsObservable();
        
        public bool IsFirstSelect { get; private set; }

        public void SelectFirst()
        {
            IsFirstSelect = true;
            EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
            IsFirstSelect = false;
        }

        public void SetTimeScaleAnimationInvalid()
        {
            continueButton.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
            restartButton.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
            returnToHubButton.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
            goToSettingsButton.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
            endGameButton.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        public void SetActiveReturnToHubButton(bool isActive)
        {
            returnToHubButton.gameObject.SetActive(isActive);
        }
        
        public void SetActiveRestartButton(bool isActive)
        {
            restartButton.gameObject.SetActive(isActive);
        }
    }
}