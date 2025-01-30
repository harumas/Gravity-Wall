using System.Collections.Generic;
using System.Linq;
using Core.Sound;
using CoreModule.Sound;
using UnityEngine;

namespace Module.Gimmick.LevelGimmick
{
    public class Switch : GimmickObject
    {
        [SerializeField] private bool initializeIsOn = false;
        [SerializeField, Tag] private List<string> targetTags;
        [SerializeField] private MeshRenderer meshRenderer, RayMeshRenderer;
        private static readonly int emissionIntensity = Shader.PropertyToID("_EmissionIntensity");
        private static readonly int powerOn = Shader.PropertyToID("_PowerOn");

        private int pushingCount = 0;

        private void Start()
        {
            if (initializeIsOn)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (targetTags.Any(tag => collider.CompareTag(tag)))
            {
                pushingCount++;
                Enable();
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            if (targetTags.Any(tag => collider.CompareTag(tag)))
            {
                pushingCount--;

                if (pushingCount == 0)
                {
                    Disable();
                }
            }
        }

        public override void Enable(bool doEffect = true)
        {
            SoundManager.Instance.Play(SoundKey.Switch, MixerType.SE);
            meshRenderer.material.SetFloat(emissionIntensity, 1.0f);
            if (RayMeshRenderer != null)
            {
                RayMeshRenderer.material.SetInt(powerOn, 0);
            }

            isEnabled.Value = true;
        }

        public override void Disable(bool doEffect = true)
        {
            meshRenderer.material.SetFloat(emissionIntensity, 0f);
            if (RayMeshRenderer != null)
            {
                RayMeshRenderer.material.SetInt(powerOn, 1);
            }

            isEnabled.Value = false;
        }

        public override void Reset()
        {
            pushingCount = 0;
            Disable(false);
        }
    }
}