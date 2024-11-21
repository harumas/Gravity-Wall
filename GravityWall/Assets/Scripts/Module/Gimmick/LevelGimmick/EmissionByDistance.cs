using System;
using System.Collections;
using System.Collections.Generic;
using CoreModule.Helper;
using UnityEngine;


namespace Gimmick.LevelGimmick
{
    public class EmissionByDistance : MonoBehaviour
    {
        [SerializeField] private Renderer targetRenderer;
        [SerializeField] private MinMaxValue distanceRange;
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

            Debug.Log(distanceRange.Remap01Squared(p.x * p.x + p.y * p.y + p.z * p.z));

            float intensity = Mathf.Max(minEmission, distanceRange.Remap01Squared(p.x * p.x + p.y * p.y + p.z * p.z));

            targetRenderer.material.SetFloat(emissionIntensityProperty, intensity);
        }
    }
}