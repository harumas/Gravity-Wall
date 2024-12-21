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

        public void SelectFirst()
        {
            EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
        }
    }
}