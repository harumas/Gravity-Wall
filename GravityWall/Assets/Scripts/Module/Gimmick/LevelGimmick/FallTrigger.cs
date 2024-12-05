using Module.Gravity;
using Module.Player;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Cinemachine;
using Module.Gimmick.SystemGimmick;
using Constants;

namespace Module.Gimmick.LevelGimmick
{
    public class FallTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject fallColliuder;
        [SerializeField] private GravitySwitchTrigger[] gravitySwitchTriggers;

        [SerializeField] private Volume volume;
        [SerializeField] private CinemachineVirtualCamera cinemachine;
        [SerializeField] private UniversalRendererData rendererData;

        bool isPlayerEnter;
        private LocalGravity targetGravity;
        private ScriptableRendererFeature feature;

        private readonly string radialBlurFeatureName = "RadialBlurFeature";

        private void Awake()
        {
            feature = rendererData.rendererFeatures.Find(f => f.name == radialBlurFeatureName);
            if (feature != null)
            {
                feature.SetActive(false);
            }
        }

        private int cameraHighPriority = 20;
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag(Tag.Player))
            {
                if (WorldGravity.Instance.Gravity == Vector3.left)
                {
                    foreach (var trigger in gravitySwitchTriggers)
                    {
                        trigger.SetEnable(false);
                    }

                    other.GetComponent<GravitySwitcher>().Disable();
                    fallColliuder.SetActive(true);
                    other.GetComponentInChildren<Animator>().SetInteger("FallIndex", 1);
                    other.GetComponent<PlayerController>().Lock();

                    if (feature != null)
                    {
                        feature.SetActive(true);
                    }

                    cinemachine.Priority = cameraHighPriority;

                    isPlayerEnter = true;
                }
            }
        }

        private readonly float GravityScale = 20;
        private void FixedUpdate()
        {
            if (isPlayerEnter)
            {
                targetGravity.SetMultiplierAtFrame(GravityScale);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(Tag.Player))
            {
                if (WorldGravity.Instance.Gravity == Vector3.left)
                {
                    GetComponent<GravitySwitchTrigger>().SetEnable(false);
                    other.GetComponent<GravitySwitcher>().Disable();
                    targetGravity = other.gameObject.GetComponent<LocalGravity>();
                }
            }
        }
    }
}