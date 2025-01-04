using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace View.View
{
    public class CreditView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Button backButton;


        public CanvasGroup CanvasGroup => canvasGroup;
        public Observable<Unit> OnBackButtonPressed => backButton.OnClickAsObservable();
        public Observable<BaseEventData> OnBackButtonSelected => backButton.OnSelectAsObservable();

        public bool IsFirstSelect { get; private set; }

        public void SelectFirst()
        {
            IsFirstSelect = true;
            EventSystem.current.SetSelectedGameObject(backButton.gameObject);
            IsFirstSelect = false;
        }
    }
}