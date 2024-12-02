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

        bool isPlayerEnter;
        private LocalGravity targetGravity;
        private MotionBlur blur;
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

                    if (volume.profile.TryGet<MotionBlur>(out blur))
                    {
                        blur.active = true;
                    }

                    cinemachine.Priority = 20;

                    isPlayerEnter = true;
                }
            }
        }

        private void FixedUpdate()
        {
            if (isPlayerEnter)
            {
                targetGravity.SetMultiplierAtFrame(20);
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