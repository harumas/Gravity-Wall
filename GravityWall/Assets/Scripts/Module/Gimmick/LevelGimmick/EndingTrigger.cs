using Constants;
using DG.Tweening;
using Module.Gimmick.SystemGimmick;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Module.Gimmick.LevelGimmick
{
    public class EndingTrigger : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private InGameEventPlayerTrap playerTrap;
        [SerializeField] private AudioSource audioSource;

        private readonly static string sceneName = "EndingMovie";

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                DOTween.To(() => audioSource.volume, (v) => audioSource.volume = v, 0, 1.0f);

                DOTween.To(() => canvasGroup.alpha, (a) => canvasGroup.alpha = a, 1, 1.0f).OnComplete(() =>
                {
                    SceneManager.LoadScene(sceneName);
                });
            }
        }
    }
}