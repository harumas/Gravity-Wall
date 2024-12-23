using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace View
{
    public class LicenseView : MonoBehaviour
    {
        [SerializeField] private Scrollbar licenseScrollbar;
        [SerializeField] private CanvasGroup canvasGroup;

        public CanvasGroup CanvasGroup => canvasGroup;

        public void SelectFirst()
        {
            licenseScrollbar.value = 1;
            EventSystem.current.SetSelectedGameObject(licenseScrollbar.gameObject);
        }
    }
}