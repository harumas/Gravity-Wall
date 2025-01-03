using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CoreModule.Helper
{
    public class FillSlider : MonoBehaviour, IDragHandler
    {
        [SerializeField] private Image fillImage;

        public void OnDrag(PointerEventData eventData)
        {
        }
    }
}