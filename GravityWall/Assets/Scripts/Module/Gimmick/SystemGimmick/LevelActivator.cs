using System.Collections.Generic;
using System.Linq;
using Constants;
using Cysharp.Threading.Tasks;
using Module.Gimmick;
using Module.Gimmick.LevelGimmick;
using R3;
using UnityEngine;

namespace Module.PlayTest
{
    public class LevelActivator : MonoBehaviour
    {
        [SerializeField] private bool startActive;
        [SerializeField] private bool activateOnOpen;
        [SerializeField] private string observeGate;
        [SerializeField] private GameObject roomObject;
        [SerializeField] private List<Gate> levelGates;

        private bool isPlayerEnter;

        public bool IsPlayerEnter => isPlayerEnter;
        public bool StartActive => startActive;

        private void Start()
        {
            if (activateOnOpen)
            {
                GimmickReference.OnGimmickReferenceUpdated += OnGimmickReferenceUpdated;
            }

            if (startActive)
            {
                foreach (Gate levelGate in levelGates)
                {
                    levelGate.UsingCount++;
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

        private void OnGimmickReferenceUpdated(GimmickReference reference)
        {
            if (reference.TryGetGimmick(observeGate, out Gate gate))
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
                    });
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
                    if (!levelGate.IsUsing)
                    {
                        levelGate.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                isPlayerEnter = true;

                if (!startActive)
                {
                    foreach (Gate levelGate in levelGates)
                    {
                        levelGate.UsingCount++;
                    }
                }

                startActive = false;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                isPlayerEnter = false;

                foreach (Gate gate in levelGates)
                {
                    gate.UsingCount--;
                }
            }
        }
    }
}