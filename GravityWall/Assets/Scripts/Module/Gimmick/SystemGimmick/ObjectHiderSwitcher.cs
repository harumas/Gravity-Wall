using System.Collections.Generic;
using Constants;
using Module.Gimmick.LevelGimmick;
using R3;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    public class ObjectHiderSwitcher : MonoBehaviour
    {
        [SerializeField] private List<Gate> gates;
        [SerializeField] private List<ObjectHider> objectHiders;
        [SerializeField] private LevelActivator levelActivator;

        private bool isPlayerEnter;

        private void Awake()
        {
            if (levelActivator.StartActive)
            {
                foreach (ObjectHider hider in objectHiders)
                {
                    hider.Enable();
                }
            }

            foreach (Gate gate in gates)
            {
                gate.IsEnabled.Subscribe(isEnabled =>
                {
                    if (isPlayerEnter && !isEnabled)
                    {
                        foreach (ObjectHider hider in objectHiders)
                        {
                            hider.Enable();
                        }
                    }
                }).AddTo(this);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                isPlayerEnter = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                isPlayerEnter = false;
            }
        }
    }
}