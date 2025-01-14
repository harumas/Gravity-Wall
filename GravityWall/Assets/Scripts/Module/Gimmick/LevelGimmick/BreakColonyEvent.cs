using System;
using Constants;
using Cysharp.Threading.Tasks;
using Module.Player;
using UnityEngine;

namespace Module.Gimmick.LevelGimmick
{
    public class BreakColonyEvent : MonoBehaviour
    {
        [SerializeField] private BrokenObject startWall, dangerWall;
        private CameraShaker cameraShaker;

        private bool wasBreak;

        private async UniTaskVoid OnBreakColony()
        {
            SetBuilding(startWall).Forget();

            wasBreak = true;

            await UniTask.Delay(TimeSpan.FromSeconds(1));

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