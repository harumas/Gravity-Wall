using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cinemachine;
using Module.PlayTest;
namespace View
{
    public class TitleView : MonoBehaviour
    {
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button continueGameButton;
        [SerializeField] private Button creditButton;
        [SerializeField] private Button endGameButton;
        [SerializeField] private StartSequencer startSequencer;
        [SerializeField] private CanvasGroup canvasGroup, fadeCanvasGroup;

        public StartSequencer StartSequencer => startSequencer;
        public CanvasGroup CanvasGroup => canvasGroup;
        public CanvasGroup FadeCanvasGroup => fadeCanvasGroup;

        public Observable<Unit> OnNewGameButtonPressed => newGameButton.OnClickAsObservable();
        public Observable<Unit> OnContinueGameButtonPressed => continueGameButton.OnClickAsObservable();
        public Observable<Unit> OnCreditButtonPressed => creditButton.OnClickAsObservable();
        public Observable<Unit> OnEndGameButtonPressed => endGameButton.OnClickAsObservable();
        public void SelectFirst()
        {
            EventSystem.current.SetSelectedGameObject(newGameButton.gameObject);
        }
    }
}