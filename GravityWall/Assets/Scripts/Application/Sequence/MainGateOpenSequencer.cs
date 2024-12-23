using System;
using Cysharp.Threading.Tasks;
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

        public void Initialize(int clearedStageCount)
        {
            EnableMainGateLights(clearedStageCount);
            
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
    }
}