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
        [SerializeField] private GameObject[] pooledLights;
        [SerializeField] private int switchMaxCount = 1;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private GimmickObject[] observedSwitches;
        [SerializeField] private float setInterval = 1f;

        private int switchCount = 0;
        private List<Material> lightMaterials = new List<Material>();
        private static readonly int emissionIntensity = Shader.PropertyToID("_EmissionIntensity");

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
                gateMeshRenderers[i].material.SetFloat(emissionIntensity, isOpen ? 1.0f : 0.0f);
            }
        }

        void ChangeCounterLights(bool isOn)
        {
            Debug.Log(isOn);
            if (isOn)
            {
                lightMaterials[switchCount].SetFloat(emissionIntensity, 1.0f);
            }
            else
            {
                lightMaterials[switchCount - 1].SetFloat(emissionIntensity, 0.0f);
            }
        }

        void GateAnimation(bool isOpen)
        {
            gateLeft.DOLocalMoveX(isOpen ? 0.9f : 0, 0.3f);
            gateRight.DOLocalMoveX(isOpen ? -0.9f : 0, 0.3f);
        }

        void InstantiateCounterLights()
        {
            if (switchMaxCount > pooledLights.Length)
            {
                Debug.LogError("SwitchMaxCount is larger than the number of pooled lights.");
                return;
            }

            float width = setInterval * (switchMaxCount - 1);

            for (int i = 0; i < switchMaxCount; i++)
            {
                var lightObj = pooledLights[i];
                lightObj.SetActive(true);
                lightObj.transform.localPosition = lightBasePosition.localPosition + Vector3.right * (setInterval * i - width / 2);
                lightMaterials.Add(lightObj.GetComponent<Renderer>().material);
            }
        }
    }
}