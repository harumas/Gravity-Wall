using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Gimmick.LevelGimmick
{
    public class EmissionByDistance : MonoBehaviour
    {
        [SerializeField] private Renderer targetRenderer;
        [SerializeField] private AnimationCurve intensityCurve;
        [SerializeField] private float multiplier;
        [SerializeField] private float minEmission;

        private Camera mainCamera;
        private static readonly int emissionIntensityProperty = Shader.PropertyToID("_EmissionIntensity");
        

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            Vector3 p = transform.position - mainCamera.transform.position;

            // 二乗則で強さを設定
            float intensity = Mathf.Max(minEmission, intensityCurve.Evaluate(p.x * p.x + p.y * p.y) * multiplier);

            targetRenderer.material.SetFloat(emissionIntensityProperty, intensity);
        }
    }
}