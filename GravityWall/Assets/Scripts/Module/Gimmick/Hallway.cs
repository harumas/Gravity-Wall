using System;
using System.Collections;
using System.Collections.Generic;
using Constants;
using R3;
using TriInspector;
using UnityEngine;

namespace Module.Gimmick
{
    public class Hallway : MonoBehaviour
    {
        [SerializeField] private GameObject hallwayBody;
        [SerializeField] private List<string> gimmickObjects;
        [SerializeField, ReadOnly] private List<Gate> referencedGates;

        private bool isPlayerEnter;
        private Gate enteredGate;

        private void Start()
        {
            GimmickReference.OnGimmickReferenceUpdated += OnGimmickReferenceUpdated;
        }

        private void OnGimmickReferenceUpdated(GimmickReference reference)
        {
            foreach (string path in gimmickObjects)
            {
                if (reference.TryGetGimmick(path, out Gate gate))
                {
                    referencedGates.Add(gate);
                }
                else
                {
                    Debug.LogWarning("Gimmick not found: " + path);
                }

                foreach (Gate referencedGate in referencedGates)
                {
                    referencedGate.IsEnabled.Skip(1).Subscribe(isEnabled => OnGateAction(gate, isEnabled)).AddTo(this);
                }
            }

            Disable(null);
        }

        private void OnGateAction(Gate gate, bool isEnabled)
        {
            if (isEnabled)
            {
                Enable();
            }
            else if (!isPlayerEnter)
            {
                Disable(gate);
            }
        }

        private void Enable()
        {
            hallwayBody.SetActive(true);

            foreach (Gate otherGate in referencedGates)
            {
                otherGate.gameObject.SetActive(true);
            }
        }

        private void Disable(Gate gate)
        {
            hallwayBody.SetActive(false);

            foreach (Gate otherGate in referencedGates)
            {
                if (!otherGate.IsUsing)
                {
                    otherGate.Disable(false);
                    otherGate.gameObject.SetActive(false);
                }
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