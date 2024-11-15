using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Module.Gimmick
{
    public class PowerPipe : MonoBehaviour
    {
        private MeshRenderer[] meshRenderers;
        // Start is called before the first frame update
        void Start()
        {
            meshRenderers = GetComponentsInChildren<MeshRenderer>();
            OnPowerPipe(false);
        }

        public void OnPowerPipe(bool isOn)
        {
            foreach (var mesh in meshRenderers)
            {
                float value = isOn ? 0.0f : 1.0f;
                DOTween.To(() => value, (v) => value = v, isOn ? 1.0f : 0.0f, 0.3f)
                .OnUpdate(() =>
                {
                    mesh.material.SetFloat("_EmissionIntensity", value);
                });
            }
        }
    }
}