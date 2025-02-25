using System;
using System.Collections.Generic;
using Constants;
using DG.Tweening;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    public class ObjectHideVolume : MonoBehaviour
    {
        [SerializeField] private SavePoint targetSavePoint;
        private static readonly int wholeDitherProperty = Shader.PropertyToID("_WholeDitherStrength");
        private Renderer[] currentRenderers;
        private Tween currentTween;
        private bool isHiding;


        private void Start()
        {
            targetSavePoint.OnSaveExecuted += () =>
            {
                if (!isHiding)
                {
                    return;
                }

                currentTween?.Kill();

                foreach (Renderer renderer in currentRenderers)
                {
                    renderer.material.SetFloat(wholeDitherProperty, 0);
                }
            };
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(Tag.HideTarget))
            {
                currentRenderers = other.gameObject.GetComponentsInChildren<Renderer>();

                float strength = 0;
                currentTween = DOTween.To(() => strength, x => strength = x, 1, 0.5f)
                    .OnUpdate(() =>
                    {
                        foreach (Renderer renderer in currentRenderers)
                        {
                            renderer.material.SetFloat(wholeDitherProperty, strength);
                        }
                    });

                isHiding = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag(Tag.HideTarget))
            {
                Renderer[] currentRenderers = other.gameObject.GetComponentsInChildren<Renderer>();

                foreach (Renderer renderer in currentRenderers)
                {
                    renderer.material.SetFloat(wholeDitherProperty, 0);
                }

                isHiding = false;
            }
        }
    }
}