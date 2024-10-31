using System.Collections.Generic;
using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.Events;

namespace Module.Gimmick
{
    public class Gate : GimmickObject
    {
        [SerializeField] private UnityEvent gateOpenEvent;
        [SerializeField] private UnityEvent gateCloseEvent;
        [SerializeField] private GameObject gate;
        [SerializeField] private MeshRenderer[] gateMeshRenderers;
        [SerializeField] private Transform lightBasePosition;
        [SerializeField] private Transform gateLeft, gateRight;
        [SerializeField] private GameObject Counterlight;
        [SerializeField] private int switchMaxCount = 1;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private GimmickObject[] observedSwitches;

        private int switchCount = 0;
        private List<Material> lightMaterials = new List<Material>();
        private const float intensity = 8.0f;

        private void Start()
        {
            InstantiateCounterLights();

            foreach (GimmickObject gimmick in observedSwitches)
            {
                gimmick.IsEnabled.Skip(1).Subscribe(UpdateGateState).AddTo(this);
            }
        }

        private void UpdateGateState(bool switchEnabled)
        {
            ChangeCounterLights(switchEnabled);

            switchCount += switchEnabled ? 1 : -1;
            bool isOpen = switchCount >= switchMaxCount;

            if (isOpen)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }

        public override void Enable(bool doEffect = true)
        {
            if (isEnabled.Value)
            {
                return;
            }

            if (doEffect)
            {
                audioSource.Play();
                GateAnimation(true);
            }

            gateOpenEvent.Invoke();
            gate.SetActive(false);
            ChangeGateLight(true);

            isEnabled.Value = true;
        }

        public override void Disable(bool doEffect = true)
        {
            if (!isEnabled.Value)
            {
                return;
            }

            if (doEffect)
            {
                audioSource.Play();
                GateAnimation(false);
            }

            gateCloseEvent.Invoke();
            gate.SetActive(true);
            ChangeGateLight(false);

            isEnabled.Value = false;
        }

        public override void Reset()
        {
            switchCount = 0;
            Disable(false);
        }

        void ChangeGateLight(bool isOpen)
        {
            for (int i = 0; i < gateMeshRenderers.Length; i++)
            {
                gateMeshRenderers[i].material.SetFloat("_EmissionIntensity", isOpen ? 1.0f : 0.0f);
            }
        }

        void ChangeCounterLights(bool isOn)
        {
            Debug.Log(isOn);
            if (isOn)
            {
                lightMaterials[switchCount].SetFloat("_EmissionIntensity", 1.0f);
            }
            else
            {
                lightMaterials[switchCount - 1].SetFloat("_EmissionIntensity", 0.0f);
            }
        }

        void GateAnimation(bool isOpen)
        {
            gateLeft.DOLocalMoveX(isOpen ? 0.9f : 0, 0.3f);
            gateRight.DOLocalMoveX(isOpen ? -0.9f : 0, 0.3f);
        }

        void InstantiateCounterLights()
        {
            if (switchMaxCount <= 1)
            {
                var light = Instantiate(Counterlight, transform);

                light.transform.localPosition = lightBasePosition.localPosition;
                light.transform.localRotation = lightBasePosition.localRotation;

                lightMaterials.Add(light.GetComponent<MeshRenderer>().material);
                return;
            }

            float width = gate.transform.localScale.x * 0.7f;
            float spacing = width / (switchMaxCount - 1);
            float startX = lightBasePosition.localPosition.x - width / 2.0f;
            for (int i = 0; i < switchMaxCount; i++)
            {
                var light = Instantiate(Counterlight, transform);

                light.transform.localPosition =
                    new Vector3(startX + i * spacing, lightBasePosition.localPosition.y, lightBasePosition.localPosition.z);
                light.transform.localRotation = lightBasePosition.localRotation;

                lightMaterials.Add(light.GetComponent<MeshRenderer>().material);
            }
        }
    }
}