using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Module.PlayTest
{
    public class StartSequencer : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera titleVirtualCamera;
        [SerializeField] private Volume volume;
        [SerializeField] private GameObject light, godray;
        public void StartSequence()
        {
            light.gameObject.SetActive(true);
            godray.gameObject.SetActive(true);
            Invoke("Event", 1.0f);
        }

        DepthOfField depth;
        void Event()
        {
            titleVirtualCamera.Priority = 0;
            if (volume.profile.TryGet<DepthOfField>(out depth))
            {
                depth.focusDistance.value = 300;
                depth.focalLength.value = 200;
            }
        }
    }
}