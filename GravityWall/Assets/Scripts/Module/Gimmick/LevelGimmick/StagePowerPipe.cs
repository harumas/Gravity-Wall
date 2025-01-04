using DG.Tweening;
using R3;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Module.Gimmick.LevelGimmick
{
    public class StagePowerPipe : GimmickObject
    {
        [SerializeField] private GimmickObject[] observedSwitches;
        private int switchMaxCount = 1;
        private int switchCount = 0;

        private MeshRenderer[] meshRenderers;
        private static readonly int emissionIntensity = Shader.PropertyToID("_EmissionIntensity");

        void Start()
        {
            meshRenderers = GetComponentsInChildren<MeshRenderer>();

            foreach (GimmickObject gimmick in observedSwitches)
            {
                gimmick.IsEnabled.Skip(1).Subscribe(UpdateMoveState).AddTo(this);
            }

            OnPowerPipe(false);
        }

        private void UpdateMoveState(bool switchEnabled)
        {
            switchCount += switchEnabled ? 1 : -1;
            bool isSwitching = switchCount >= switchMaxCount;

            if (isSwitching)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }

        public void OnPowerPipe(bool isOn)
        {
            float value = isOn ? 0.0f : 1.0f;
            DOTween.To(() => value, (v) => value = v, isOn ? 1.0f : 0.0f, 0.3f)
                .OnUpdate(() =>
                {
                    foreach (var mesh in meshRenderers)
                    {
                        mesh.material.SetFloat(emissionIntensity, value);
                    }
                });
        }

        public override void Enable(bool doEffect = true)
        {
            OnPowerPipe(true);
        }

        public override void Disable(bool doEffect = true)
        {
            OnPowerPipe(false);
        }

        public override void Reset()
        {
            OnPowerPipe(false);
        }
    }
}