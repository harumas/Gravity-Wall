using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Module.Gimmick.SystemGimmick;
using UnityEngine.SceneManagement;
using CoreModule.Sound;

namespace Module.LevelGimmick
{
    public class EndingTrigger : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private InGameEventPlayerTrap playerTrap;

        private readonly static string sceneName = "EndingMovie";

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                SoundManager.Instance.Pause(1.0f);

                DOTween.To(() => canvasGroup.alpha,(a) => canvasGroup.alpha = a,1,1.0f).OnComplete(() =>
                {
                    SceneManager.LoadScene(sceneName);
                });
            }
        }
    }
}