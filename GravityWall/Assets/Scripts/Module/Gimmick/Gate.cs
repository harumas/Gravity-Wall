using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Module.Gimmick
{
    public class Gate : AbstractGimmickAffected
    {
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
            if (switchMaxCount <= 1)
            {
                Debug.LogWarning("オブジェクトの数が2つ以上必要です。");
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

        public override void Affect(AbstractSwitch switchObject)
        {
            switchCount += switchObject.isOn ? 1 : -1;
            isOpen = switchCount >= switchMaxCount;
            gate.SetActive(!isOpen);

            Debug.Log(switchCount);

            lightMaterials[switchCount - 1].EnableKeyword("_EMISSION");
            lightMaterials[switchCount - 1].SetColor("_EmissionColor", Color.green * intensity);
        }
    }
}