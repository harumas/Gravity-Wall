using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Module.Gravity;
using DG.Tweening;
using Constants;
namespace Module.Gimmick
{
    public class BreakGlass : MonoBehaviour
    {
        [SerializeField] private GameObject glass, breakedGlass;
        [SerializeField] private Collider collider;
        [SerializeField] private PlayableDirector director;
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
                    Time.timeScale = 0.1f;
                    Time.fixedDeltaTime = 0.002f;
                    collider.enabled = false;

                    breakedGlass.transform.localScale = scale;
                    breakedGlass.transform.DOScaleZ(0.7f, 2.0f).SetUpdate(true).OnComplete(() =>
                    {
                        director.Play();
                    });
                }
            }
        }
    }
}