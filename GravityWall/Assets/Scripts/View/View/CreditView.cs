using UnityEngine;

namespace View
{
    public class CreditView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        
        public CanvasGroup CanvasGroup => canvasGroup;
    }
}