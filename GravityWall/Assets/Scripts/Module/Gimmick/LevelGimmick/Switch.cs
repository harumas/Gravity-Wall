using System.Collections.Generic;
using System.Linq;
using Core.Sound;
using CoreModule.Sound;
using PropertyGenerator.Generated;
using UnityEngine;

namespace Module.Gimmick.LevelGimmick
{
    public class Switch : GimmickObject
    {
        [SerializeField] private bool initializeIsOn = false;
        [SerializeField, Tag] private List<string> targetTags;
        [SerializeField] private EmissionObjectWrapper batteryStandShaderWrapper;
        [SerializeField] private PowerCubeLightShaderWrapper rayShaderWrapper;

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
            batteryStandShaderWrapper.EmissionIntensity = 1.0f;
            rayShaderWrapper.PowerOn = 0f;
            isEnabled.Value = true;
        }

        public override void Disable(bool doEffect = true)
        {
            batteryStandShaderWrapper.EmissionIntensity = 0f;
            rayShaderWrapper.PowerOn = 1f;
            isEnabled.Value = false;
        }

        public override void Reset()
        {
            pushingCount = 0;
            Disable(false);
        }
    }
}