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

        [SerializeField] private AudioSource audioSource;

        bool isPlayerEnter;
        private LocalGravity targetGravity;
        private ScriptableRendererFeature feature;


        private readonly string radialBlurFeatureName = "RadialBlurFeature";
        private readonly string cameraPivotName = "CameraPivot";

        private readonly string animatorFallIndexName = "FallIndex";
        private readonly int fallIndex = 1;

        private readonly int cameraHighPriority = 20;
        private readonly float GravityScale = 30;

        private DepthOfField depth;

        private void Awake()
        {
            feature = rendererData.rendererFeatures.Find(f => f.name == radialBlurFeatureName);
            if (feature != null)
            {
                feature.SetActive(false);
            }
        }

        private readonly float depthFocusDistance = 200;
        private readonly float depthFocalLength = 120;
        private readonly float defaultDepthAperture = 17;

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

                    if (volume.profile.TryGet<DepthOfField>(out depth))
                    {
                        depth.focusDistance.value = depthFocusDistance;
                        depth.focalLength.value = depthFocalLength;
                        depth.aperture.value = defaultDepthAperture;
                    }

                    fallCollider.SetActive(true);

                    SetPlayerSettings(other.transform);

                    SetCameraPivot(other.transform.Find(cameraPivotName));

                    audioSource.Play();

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
            
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.Lock();
            playerController.HoldLock = true;
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

        private void OnDestroy()
        {
            if (feature != null)
            {
                feature.SetActive(false);
            }
        }
    }
}