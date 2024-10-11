using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Module.Gravity;
using DG.Tweening;
using Constants;
using UnityEngine.SceneManagement;
namespace Module.Gimmick
{
    public class BreakGlass : MonoBehaviour
    {
        [SerializeField] private GameObject glass, breakedGlass;
        [SerializeField] private Collider collider;
        [SerializeField] private PlayableDirector director;
        [SerializeField] private GameObject clearCanvas;
        [SerializeField] private AudioSource audioSource;
        Vector3 scale;

        void Start()
        {
            scale = breakedGlass.transform.localScale;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag(Tag.Player))
            {
                if (WorldGravity.Instance.Gravity == Vector3.left)
                {
                    glass.SetActive(false);
                    breakedGlass.SetActive(true);
                    Time.timeScale = 0.05f;
                    Time.fixedDeltaTime = 0.001f;
                    collider.enabled = false;

                    clearCanvas.SetActive(true);

                    audioSource.Play();

                    breakedGlass.transform.localScale = scale;
                    breakedGlass.transform.DOScaleZ(0.7f, 3.0f).SetUpdate(true).OnComplete(() =>
                    {
                        Time.timeScale = 1.0f;
                        Time.fixedDeltaTime = 0.01f;
                        SceneManager.LoadScene("StageSelect");
                    });
                }
            }
        }
    }
}