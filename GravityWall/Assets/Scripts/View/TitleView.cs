using R3;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class TitleView : MonoBehaviour
    {
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueGameButton;
        [SerializeField] private Button licenseButton;
        [SerializeField] private Button endGameButton;

        public Observable<Unit> OnNewGameButtonPressed => newGameButton.OnClickAsObservable();
        public Observable<Unit> OnContinueGameButtonPressed => continueGameButton.OnClickAsObservable();
        public Observable<Unit> OnLicenseButtonPressed => licenseButton.OnClickAsObservable();
        public Observable<Unit> OnEndGameButtonPressed => endGameButton.OnClickAsObservable();
    }
}