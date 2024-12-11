using UnityEngine;
using UnityEngine.Playables;
using Module.Gravity;
using DG.Tweening;
using Constants;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Events;

namespace Module.Gimmick.LevelGimmick
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
                    BreakGlassEffect();
                }
            }
        }


        private readonly float timeScale = 0.05f;
        private readonly float fixedDeltaTime = 0.001f;
        private readonly float defaultFixedDeltaTime = 0.01f;
        private readonly float defaultTimeScale = 1.0f;
        void BreakGlassEffect()
        {
            glass.SetActive(false);
            breakedGlass.SetActive(true);
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = fixedDeltaTime;
            collider.enabled = false;

            onClear.Invoke();

            audioSource.Play();

            breakedGlass.transform.localScale = scale;
            breakedGlass.transform.DOScaleZ(0.7f, 1.0f)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                Time.timeScale = defaultTimeScale;
                Time.fixedDeltaTime = defaultFixedDeltaTime;
            });
        }
    }
}