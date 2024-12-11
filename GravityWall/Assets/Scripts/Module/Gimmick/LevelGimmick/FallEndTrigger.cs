using Cinemachine;
using Constants;
using Cysharp.Threading.Tasks;
using Module.Player;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Module.Gimmick
{
    public class FallEndTrigger : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera cinemachine;
        [SerializeField] private Volume volume;
        [SerializeField] private GameObject fallTrigger;
        [SerializeField] private UniversalRendererData rendererData;

        private ScriptableRendererFeature feature;

        private readonly string blurFeatureName = "RadialBlurFeature";
        private readonly string animatorFallIndexName = "FallIndex";
        private readonly int fallIndex = 2;
        private readonly int playerControlUnlockDelay = 300;


        private void Start()
        {
            feature = rendererData.rendererFeatures.Find(f => f.name == blurFeatureName);
            if (feature != null)
            {
                feature.SetActive(false);
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(Tag.Player))
            {
                cinemachine.Priority = 0;
                if (feature != null)
                {
                    feature.SetActive(false);
                }

                fallTrigger.SetActive(false);

                FallEndSequence(other.gameObject).Forget();
            }
        }

        private async UniTaskVoid FallEndSequence(GameObject player)
        {
            player.GetComponentInChildren<Animator>().SetInteger(animatorFallIndexName, fallIndex);

            await UniTask.Delay(playerControlUnlockDelay);

            player.GetComponent<GravitySwitcher>().Enable();
            player.GetComponent<PlayerController>().Unlock();
        }
    }
}