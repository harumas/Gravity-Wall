using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Module.Gimmick
{
    public class Switch : GimmickObject
    {
        [SerializeField] private bool initializeIsOn = false;
        [SerializeField, Tag] private List<string> targetTags;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private MeshRenderer meshRenderer, RayMeshRenderer;

        private int pushingCount = 0;

        private void Start()
        {
            if (initializeIsOn)
            {
                Enable();
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
            audioSource.Play();
            meshRenderer.material.SetFloat("_EmissionIntensity", 1.0f);
            RayMeshRenderer.material.SetInt("_PowerOn", 0);
            isEnabled.Value = true;
        }

        public override void Disable(bool doEffect = true)
        {
            //offEvent.Invoke();
            meshRenderer.material.SetFloat("_EmissionIntensity", 0f);
            RayMeshRenderer.material.SetInt("_PowerOn", 1);
            isEnabled.Value = false;
        }

        public override void Reset()
        {
            pushingCount = 0;
            Disable(false);
        }
    }
}