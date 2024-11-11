using R3;
using UnityEngine;
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
        public Observable<Unit> OnReturnToHubButton => returnToHubButton.OnClickAsObservable();
        public Observable<Unit> OnGoToSettingsButtonPressed => goToSettingsButton.OnClickAsObservable();
        public Observable<Unit> OnEndGameButtonPressed => endGameButton.OnClickAsObservable();
    }
}