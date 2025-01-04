using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Application
{
    public class SplashScreen : MonoBehaviour
    {
        [SerializeField] private Image splashImage;
        [SerializeField] private float fadeDelay;
        [SerializeField] private float showDuration;
        [SerializeField] private float fadeDuration;
        [SerializeField] private GameObject destroyTarget;

        private void Awake()
        {
            Color color = splashImage.color;
            color.a = 0f;
            splashImage.color = color;
        }

        public DOTweenAsyncExtensions.TweenAwaiter Show()
        {
            DG.Tweening.Sequence sequence = DOTween.Sequence();
            sequence.Append(splashImage.DOFade(1f, fadeDuration).SetDelay(fadeDelay).SetAutoKill(false));
            sequence.AppendInterval(showDuration);
            
            return sequence.GetAwaiter();
        }

        public void Destroy()
        {
            Destroy(destroyTarget);
        }
    }
}