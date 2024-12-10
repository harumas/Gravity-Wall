using Cinemachine;
using Constants;
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

        private void Start()
        {
            feature = rendererData.rendererFeatures.Find(f => f.name == blurFeatureName);
            if (feature != null)
            {
                feature.SetActive(false);
            }
        }

        private int minCameraPriority = 0;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(Tag.Player))
            {
                cinemachine.Priority = minCameraPriority;

                if (feature != null)
                {
                    feature.SetActive(false);
                }

                other.GetComponent<GravitySwitcher>().Enable();
                other.GetComponentInChildren<Animator>().SetInteger("FallIndex", 2);
                UnlockPlayerMove(other.GetComponent<PlayerController>());
                fallTrigger.SetActive(false);
            }
        }

        void UnlockPlayerMove(PlayerController player)
        {
            player.Unlock();
        }
    }
}