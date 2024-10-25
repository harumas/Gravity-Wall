using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Module.Gimmick
{
    public class Gate : AbstractGimmickAffected
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
        private int switchCount = 0;
        private List<Material> lightMaterials = new List<Material>();
        public bool isOpen { get; private set; }
        private const float intensity = 8.0f;

        private void Start()
        {
            InstantiateCounterLights();
        }

        public override void Affect(AbstractSwitch switchObject)
        {
            ChangeCounterLights(switchObject.isOn);

            switchCount += switchObject.isOn ? 1 : -1;

            if (isOpen && switchCount < switchMaxCount)
            {
                gateCloseEvent.Invoke();
                audioSource.Play();
                GateAnimation(false);
            }

            isOpen = switchCount >= switchMaxCount;
            gate.SetActive(!isOpen);

            ChangeGateLight(isOpen);

            if (isOpen)
            {
                Debug.Log("isOpenNow");
                gateOpenEvent.Invoke();
                audioSource.Play();
                GateAnimation(true);
            }
        }

        public override void Reset()
        {
            switchCount = 0;
            isOpen = false;
            gate.SetActive(!isOpen);
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

                light.transform.localPosition = new Vector3(startX + i * spacing, lightBasePosition.localPosition.y, lightBasePosition.localPosition.z);
                light.transform.localRotation = lightBasePosition.localRotation;

                lightMaterials.Add(light.GetComponent<MeshRenderer>().material);
            }
        }
    }
}