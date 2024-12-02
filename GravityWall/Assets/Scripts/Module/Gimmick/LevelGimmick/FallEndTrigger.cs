using System.Collections;
using System.Collections.Generic;
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

        private MotionBlur blur;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(Tag.Player))
            {
                cinemachine.Priority = 0;

                if (volume.profile.TryGet<MotionBlur>(out blur))
                {
                    blur.active = false;
                }

                other.GetComponent<GravitySwitcher>().Enable();
                other.GetComponentInChildren<Animator>().SetInteger("FallIndex", 2);
                other.GetComponent<PlayerController>().Unlock();

                fallTrigger.SetActive(false);
            }
        }
    }
}