using System.Collections.Generic;
using System.Linq;
using Constants;
using Cysharp.Threading.Tasks;
using Module.Gimmick;
using R3;
using UnityEngine;

namespace Module.PlayTest
{
    public class LevelActivator : MonoBehaviour
    {
        [SerializeField] private bool startActive;
        [SerializeField] private GameObject roomObject;
        [SerializeField] private List<Gate> levelGates;
        [SerializeField] private List<ObjectHider> objectHiders;

        private bool isPlayerEnter;

        private void Start()
        {
            if (startActive)
            {
                foreach (Gate levelGate in levelGates)
                {
                    levelGate.IsUsing = true;
                }

                Activate();
            }
            else
            {
                Deactivate();
            }

            foreach (Gate gate in levelGates)
            {
                gate.IsEnabled.Skip(1)
                    .Subscribe(isEnabled =>
                    {
                        if (isEnabled)
                        {
                            Activate();
                        }
                        else
                        {
                            Deactivate();
                        }
                    })
                    .AddTo(this);
            }
        }

        public void Activate()
        {
            if (roomObject != null)
            {
                roomObject.SetActive(true);
            }

            foreach (Gate levelGate in levelGates)
            {
                levelGate.gameObject.SetActive(true);
            }
        }

        public void Deactivate()
        {
            bool allDisabled = levelGates.All(g => !g.IsEnabled.CurrentValue);

            if (allDisabled && !isPlayerEnter)
            {
                if (roomObject != null)
                {
                    roomObject.SetActive(false);
                }

                foreach (Gate levelGate in levelGates)
                {
                    levelGate.gameObject.SetActive(false);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                isPlayerEnter = true;

                foreach (Gate levelGate in levelGates)
                {
                    levelGate.IsUsing = true;
                }

                foreach (ObjectHider hider in objectHiders)
                {
                    hider.Enable();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                isPlayerEnter = false;

                foreach (Gate gate in levelGates)
                {
                    gate.IsUsing = false;
                }
            }
        }
    }
}