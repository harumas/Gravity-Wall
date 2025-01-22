using System;
using System.Collections.Generic;
using Constants;
using DG.Tweening;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    public class ObjectHideVolume : MonoBehaviour
    {
        private static readonly int wholeDitherProperty = Shader.PropertyToID("_WholeDitherStrength");

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(Tag.HideTarget))
            {
                Renderer[] renderers = other.gameObject.GetComponentsInChildren<Renderer>();

                float strength = 0;
                DOTween.To(() => strength, x => strength = x, 1, 0.5f)
                    .OnUpdate(() =>
                    {
                        foreach (Renderer renderer in renderers)
                        {
                            renderer.material.SetFloat(wholeDitherProperty, strength);
                        }
                    });
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag(Tag.HideTarget))
            {
                Renderer[] renderers = other.gameObject.GetComponentsInChildren<Renderer>();

                foreach (Renderer renderer in renderers)
                {
                    renderer.material.SetFloat(wholeDitherProperty, 0);
                }
            }
        }
    }
}