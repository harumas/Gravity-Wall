using System;
using Cysharp.Threading.Tasks;
using Module.Gimmick;
using Module.Gimmick.LevelGimmick;
using R3;
using UnityEngine;
using UnityEngine.Playables;

namespace Application.Sequence
{
    public class MainGateOpenSequencer : MonoBehaviour
    {
        [SerializeField] private MainGate mainGate;
        [SerializeField] private PressurePlate mainGateSwitch;
        [SerializeField] private PlayableDirector mainGateOpenDirector;
        [SerializeField] private HubHologramSetter[] hubHologramSetters;

        public void Initialize(bool[] isClearedStages)
        {
            int clearedStageCount = 0;
            foreach (bool isClearedStage in isClearedStages)
            {
                if (!isClearedStage) continue;
                clearedStageCount++;
            }

            EnableMainGateLights(clearedStageCount);
            SetHologram(isClearedStages);

            mainGateSwitch.IsEnabled.Subscribe(isEnabled =>
            {
                if (isEnabled)
                {
                    if (mainGate.CanOpen)
                    {
                        mainGateOpenDirector.Play();
                    }
                    else
                    {
                        mainGateSwitch.Disable();
                    }
                }
            }).AddTo(this);

            EnableSwitch();
            mainGate.OnLightEnabled += EnableSwitch;
        }

        private void EnableMainGateLights(int lightCount)
        {
            for (int i = 0; i < lightCount; i++)
            {
                mainGate.EnableLight();
            }
        }

        private void EnableSwitch()
        {
            if (mainGate.CanOpen)
            {
                mainGateSwitch.Unlock();
            }
        }

        private void SetHologram(bool[] isClearedStages)
        {
            for (int i = 0; i < hubHologramSetters.Length; i++)
            {
                hubHologramSetters[i].SetHologramMaterial(isClearedStages[i] == true);
            }
        }
    }
}