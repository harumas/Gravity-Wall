using System;
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
        [SerializeField] private List<GimmickObject> levelGates;

        private bool isPlayerEnter;

        private void Start()
        {
            roomObject.SetActive(startActive);

            foreach (GimmickObject gate in levelGates)
            {
                gate.IsEnabled.Subscribe(isEnabled =>
                    {
                        if (isEnabled)
                        {
                            roomObject.SetActive(true);
                        }
                        else
                        {
                            bool allDisabled = levelGates.All(g => !g.IsEnabled.CurrentValue);

                            if (allDisabled && !isPlayerEnter)
                            {
                                roomObject.SetActive(false);
                            }
                        }
                    })
                    .AddTo(destroyCancellationToken);
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