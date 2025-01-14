using Constants;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using Module.Player;
using Module.Gimmick.LevelGimmick;

namespace Module.Gimmick
{
    public class BreakColonyEvent : MonoBehaviour
    {
        [SerializeField] private BrokenObject startWall, dangerWall, breakGlassHouse;
        [SerializeField] private AudioSource audioSource;
        private CameraShaker cameraShaker;

        private bool wasBreak;

        private async UniTaskVoid OnBreakColony()
        {
            audioSource.Play();
            SetBuilding(breakGlassHouse).Forget();
            SetBuilding(startWall).Forget();

            wasBreak = true;

            await UniTask.Delay(TimeSpan.FromSeconds(1));

            audioSource.Play();
            SetBuilding(dangerWall).Forget();
        }

        private async UniTaskVoid SetBuilding(BrokenObject building)
        {
            await building.DoMove();

            cameraShaker.ShakeCamera(0.5f, 1);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (wasBreak)
            {
                return;
            }

            if (other.CompareTag(Tag.Player))
            {
                OnBreakColony().Forget();
                cameraShaker = other.GetComponent<CameraShaker>();
            }
        }
    }
}