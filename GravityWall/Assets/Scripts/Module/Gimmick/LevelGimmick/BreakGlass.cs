using UnityEngine;
using UnityEngine.Playables;
using Module.Gravity;
using DG.Tweening;
using Constants;
using UnityEngine.Events;
using UnityEngine.Rendering;//Volumeを使うのに必要
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;//Bloomを使うのに必要

namespace Module.Gimmick
{
    public class BreakGlass : MonoBehaviour
    {
        [SerializeField] private GameObject glass, breakedGlass;
        [SerializeField] private Collider collider;
        [SerializeField] private PlayableDirector director;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private UnityEvent onClear;
        [SerializeField] private Volume volume;
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
                    DepthOfField depth;
                    if (volume.profile.TryGet<DepthOfField>(out depth))
                    {
                        depth.active = false;
                    }

                    glass.SetActive(false);
                    breakedGlass.SetActive(true);
                    Time.timeScale = 0.05f;
                    Time.fixedDeltaTime = 0.001f;
                    collider.enabled = false;

                    onClear.Invoke();

                    audioSource.Play();

                    breakedGlass.transform.localScale = scale;
                    breakedGlass.transform.DOScaleZ(0.7f, 3.0f)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        Time.timeScale = 1.0f;
                        Time.fixedDeltaTime = 0.01f;
                        SceneManager.LoadScene("Test_stairs_01");
                    });
                }
            }
        }
    }
}