using Constants;
using Module.Gimmick.LevelGimmick;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

namespace Module.LevelGimmick
{
    public class ColonyHologramTrigger : MonoBehaviour
    {
        [SerializeField] private EndingHologramSetter[] endingHologramSetters;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                OnHologram().Forget();
            }
        }

        private async UniTaskVoid OnHologram()
        {
            int index = 0;
            float[] delayArray = { 0.2f,0.1f, 0.2f, 0.2f, 0.1f, 0.2f, 0.3f, 0.1f, 0.2f };
            foreach (var holo in endingHologramSetters)
            {
                holo.SetHologram();

                await UniTask.Delay(TimeSpan.FromSeconds(delayArray[index++]));
            }
        }
    }
}