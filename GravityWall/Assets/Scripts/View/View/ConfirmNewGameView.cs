using R3;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace View
{
    public class ConfirmNewGameView : MonoBehaviour
    {
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        public Observable<Unit> OnConfirmButtonPressed => confirmButton.OnClickAsObservable();
        public Observable<Unit> OnCancelButtonPressed => cancelButton.OnClickAsObservable();
        public bool IsFirstSelect { get; private set; }

        public void SelectFirst()
        {
            IsFirstSelect = true;
            EventSystem.current.SetSelectedGameObject(cancelButton.gameObject);
            IsFirstSelect = false;
        }
    }
}