using Constants;
using Cysharp.Threading.Tasks;
using Module.Player;
using UnityEngine;

namespace Module.Gimmick.LevelGimmick
{
    public class BreakHouseEvent : MonoBehaviour
    {
        [SerializeField] private BrokenObject breakGlassHouse;
        [SerializeField] private AudioSource audioSource;
        private CameraShaker cameraShaker;
        private bool wasBreak;

        private void OnBreakColony()
        {
            SetBuilding(breakGlassHouse).Forget();

            wasBreak = true;
        }

        private async UniTaskVoid SetBuilding(BrokenObject building)
        {
            audioSource.Play();
            
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
                OnBreakColony();
                cameraShaker = other.GetComponent<CameraShaker>();
            }
        }
    }
}