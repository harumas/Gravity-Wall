using System;
using Constants;
using CoreModule.Sound;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Module.Gimmick.LevelGimmick
{
    public class ColonyHologramTrigger : MonoBehaviour
    {
        [SerializeField] private EndingHologramSetter[] endingHologramSetters;
        [SerializeField] private StageLocker stageLocker;
        private bool wasHologram = false;

        private void OnTriggerEnter(Collider other)
        {
            if (wasHologram) return;

            if (other.CompareTag(Tag.Player))
            {
                OnHologram().Forget();
                stageLocker.Lock();
                wasHologram = true;
            }
        }

        private async UniTaskVoid OnHologram()
        {
            int index = 0;
            float[] delayArray = { 0.2f,0.1f, 0.2f, 0.2f, 0.1f, 0.2f, 0.3f, 0.1f, 0.2f };
            foreach (var holo in endingHologramSetters)
            {
                holo.SetHologram();
                SoundManager.Instance.Play(Core.Sound.SoundKey.OpenLock,Core.Sound.MixerType.SE);

                await UniTask.Delay(TimeSpan.FromSeconds(delayArray[index++]));
            }
        }
    }
}