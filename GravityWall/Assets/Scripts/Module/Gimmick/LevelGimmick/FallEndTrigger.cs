using Cinemachine;
using Constants;
using CoreModule.Sound;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Module.Player;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Module.Gimmick.LevelGimmick
{
    public class FallEndTrigger : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera cinemachine;
        [SerializeField] private GameObject fallTrigger;
        [SerializeField] private UniversalRendererData rendererData;
        [SerializeField] private GameObject[] tutorialObjects;
        [SerializeField] private AudioSource audioSource;

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
            if (!other.gameObject.CompareTag(Tag.Player))
            {
                return;
            }

            cinemachine.Priority = 0;
            if (feature != null)
            {
                feature.SetActive(false);
            }

            fallTrigger.SetActive(false);

            FallEndSequence(other.gameObject).Forget();
        }

        private async UniTaskVoid FallEndSequence(GameObject player)
        {
            player.GetComponentInChildren<Animator>().SetInteger(animatorFallIndexName, fallIndex);

            DOTween.To(() => audioSource.volume, (v) => audioSource.volume = v, 0, 1.0f).OnComplete(() =>
            {
                audioSource.Stop();
            });

            SoundManager.Instance.Play(Core.Sound.SoundKey.Jump,Core.Sound.MixerType.SE);

            await UniTask.Delay(playerControlUnlockDelay, cancellationToken: destroyCancellationToken);

            player.GetComponent<GravitySwitcher>().Enable();

            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.HoldLock = false;
            playerController.Unlock();

            foreach (GameObject tutorialObject in tutorialObjects)
            {
                await UniTask.DelayFrame(2, cancellationToken: destroyCancellationToken);
                Destroy(tutorialObject);
            }

            Resources.UnloadUnusedAssets();
        }
    }
}