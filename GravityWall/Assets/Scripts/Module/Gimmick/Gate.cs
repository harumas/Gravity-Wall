using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Module.Gimmick
{
    public class Gate : AbstractGimmickAffected
    {
        [SerializeField] private UnityEvent gateOpenEvent;
        [SerializeField] private GameObject gate;
        [SerializeField] private Transform lightBasePosition;
        [SerializeField] private GameObject Counterlight;
        [SerializeField] private int switchMaxCount = 1;
        private int switchCount = 0;
        private List<Material> lightMaterials = new List<Material>();
        private bool isOpen;

        private const float intensity = 8.0f;
        private void Start()
        {
            InstantiateCounterLights();
        }

        public override void Affect(AbstractSwitch switchObject)
        {
            ChangeCounterLights(switchObject.isOn);

            switchCount += switchObject.isOn ? 1 : -1;
            isOpen = switchCount >= switchMaxCount;
            gate.SetActive(!isOpen);

            if (isOpen)
            {
                gateOpenEvent.Invoke();
            }

            Debug.Log(switchCount);
        }

        void ChangeCounterLights(bool isOn)
        {
            if (isOn)
            {
                lightMaterials[switchCount].EnableKeyword("_EMISSION");
                lightMaterials[switchCount].SetColor("_EmissionColor", Color.green * intensity);
            }
            else
            {
                lightMaterials[switchCount - 1].DisableKeyword("_EMISSION");
            }
        }

        void InstantiateCounterLights()
        {
            if (switchMaxCount <= 1)
            {
                var light = Instantiate(Counterlight, transform);
                light.transform.localPosition = lightBasePosition.localPosition;
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
                lightMaterials.Add(light.GetComponent<MeshRenderer>().material);
            }
        }
    }
}