using System;
using PropertyGenerator.Generated;
using UnityEngine;

namespace Module.Gimmick.LevelGimmick
{
    public class MainGate : GimmickObject
    {
        [SerializeField] private HubGateWrapper hubGateWrapper;
        [SerializeField] private Renderer[] lightRenderers;
        [SerializeField, ColorUsage(false, true)] private Color enableLightColor;
        [SerializeField, ColorUsage(false, true)] private Color disableLightColor;
        [SerializeField] private int minEnableCount;
        [SerializeField] private int enableCount;

        public bool CanOpen => enableCount >= minEnableCount;
        public event Action OnLightEnabled;
        private static readonly int emissionColorProperty = Shader.PropertyToID("_EmissionColor");
        private static readonly string lightEffectName = "OpenEffect";

        public void EnableLight()
        {
            if (enableCount >= lightRenderers.Length)
            {
                return;
            }

            lightRenderers[enableCount].material.SetColor(emissionColorProperty, enableLightColor);
            var lightEffect = lightRenderers[enableCount].transform.Find(lightEffectName).gameObject;
            lightEffect.SetActive(true);

            enableCount++;

            OnLightEnabled?.Invoke();
        }

        public override void Enable(bool doEffect = true)
        {
            hubGateWrapper.IsOpen = true;
        }

        public override void Disable(bool doEffect = true)
        {
            hubGateWrapper.IsOpen = false;

            foreach (Renderer lightRenderer in lightRenderers)
            {
                lightRenderer.material.color = disableLightColor;
            }

            enableCount = 0;
        }

        public override void Reset() { }
    }
}