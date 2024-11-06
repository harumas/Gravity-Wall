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

        private void Awake()
        {
            foreach (string path in gimmickObjects)
            {
                if (GimmickReference.TryGetGimmick(path, out Gate gate))
                {
                    referencedGates.Add(gate);
                    gate.IsEnabled.Subscribe(isEnabled => OnGateAction(gate, isEnabled)).AddTo(this);
                }
            }
        }

        private void OnGateAction(Gate gate, bool isEnabled)
        {
            if (isEnabled)
            {
                hallwayBody.SetActive(true);
                enteredGate = gate;
            }
            else if (!isPlayerEnter)
            {
                hallwayBody.SetActive(false);

                foreach (Gate otherGate in referencedGates)
                {
                    if (gate != otherGate)
                    {
                        otherGate.gameObject.SetActive(false);
                    }
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