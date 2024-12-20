using R3;
using UnityEngine;
using UnityEngine.UI;

namespace View
{
    public class ConfirmNewGameView : MonoBehaviour
    {
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        public Observable<Unit> OnConfirmButtonPressed => confirmButton.OnClickAsObservable();
        public Observable<Unit> OnCancelButtonPressed => cancelButton.OnClickAsObservable();
    }
}