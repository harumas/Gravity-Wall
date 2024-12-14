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
        [SerializeField] private GameObject fallCollider;
        [SerializeField] private GravitySwitchTrigger[] gravitySwitchTriggers;

        [SerializeField] private Volume volume;
        [SerializeField] private CinemachineVirtualCamera cinemachine;
        [SerializeField] private UniversalRendererData rendererData;

        bool isPlayerEnter;
        private LocalGravity targetGravity;
        private ScriptableRendererFeature feature;


        private readonly string radialBlurFeatureName = "RadialBlurFeature";
        private readonly string cameraPivotName = "CameraPivot";
        
        private readonly string animatorFallIndexName = "FallIndex";
        private readonly int fallIndex = 1;

        private readonly int cameraHighPriority = 20;
        private readonly float GravityScale = 20;

        private void Awake()
        {
            feature = rendererData.rendererFeatures.Find(f => f.name == radialBlurFeatureName);
            if (feature != null)
            {
                feature.SetActive(false);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag(Tag.Player))
            {
                if (WorldGravity.Instance.Gravity == Vector3.forward)
                {
                    foreach (var trigger in gravitySwitchTriggers)
                    {
                        trigger.SetEnable(false);
                    }

                    fallCollider.SetActive(true);

                    SetPlayerSettings(other.transform);

                    SetCameraPivot(other.transform.Find(cameraPivotName));

                    cinemachine.Priority = cameraHighPriority;
                    if (feature != null)
                    {
                        feature.SetActive(true);
                    }

                    isPlayerEnter = true;
                }
            }
        }

        void SetCameraPivot(Transform pivot)
        {
            cinemachine.LookAt = pivot;
            cinemachine.Follow = pivot;
        }

        void SetPlayerSettings(Transform player)
        {
            player.GetComponent<GravitySwitcher>().Disable();
            player.GetComponentInChildren<Animator>().SetInteger(animatorFallIndexName, fallIndex);
            player.GetComponent<PlayerController>().Lock();
        }

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
                if (WorldGravity.Instance.Gravity == Vector3.forward)
                {
                    GetComponent<GravitySwitchTrigger>().SetEnable(false);
                    other.GetComponent<GravitySwitcher>().Disable();
                    targetGravity = other.gameObject.GetComponent<LocalGravity>();
                }
            }
        }
    }
}