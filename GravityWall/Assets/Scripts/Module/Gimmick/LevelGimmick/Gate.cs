using System.Collections.Generic;
using System.Media;
using Core.Sound;
using CoreModule.Sound;
using DG.Tweening;
using R3;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Module.Gimmick.LevelGimmick
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
        [SerializeField] private GimmickObject[] observedSwitches;
        [SerializeField] private float setWidth = 2.7f;
        [SerializeField, ReadOnly] private int usingCount = 0;
        [SerializeField] private Material lockHoloMaterial, openHoloMaterial;
        [SerializeField] private MeshRenderer hologramMeshRenderer;
        [SerializeField] private bool isEffect;

        private int switchCount = 0;
        private List<Material> lightMaterials = new List<Material>();
        private Tween closeTween;
        private static readonly int emissionColor = Shader.PropertyToID("_EmissionColor");
        private static readonly int alphaProperty = Shader.PropertyToID("_Alpha");

        private Color green = new Color(1.2f, 12f, 7);
        private Color red = new Color(12f, 1.1f, 2);

        public bool IsUsing => UsingCount > 0;

        public int UsingCount
        {
            get => usingCount;
            set
            {
                usingCount = value;
                usingCount = Mathf.Max(usingCount, 0);
            }
        }


        private void Start()
        {
            InstantiateCounterLights();
            ChangeGateLight(false);
            foreach (GimmickObject gimmick in observedSwitches)
            {
                gimmick.IsEnabled.Skip(1).Subscribe(UpdateGateState).AddTo(this);
            }

            if (isEnabled.Value)
            {
                SetGate(true);
                gateOpenEvent.Invoke();
                gate.SetActive(false);
                ChangeGateLight(true);
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
                SoundManager.Instance.Play(SoundKey.GateOpen, MixerType.SE);
                GateAnimation(true);
            }
            else
            {
                SetGate(true);
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
            
            closeTween?.Kill();

            if (doEffect)
            {
                SoundManager.Instance.Play(SoundKey.GateOpen, MixerType.SE);
                GateAnimation(false);
            }
            else
            {
                SetGate(false);
            }

            hologramMeshRenderer.material.SetFloat(alphaProperty, 0.2f);

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
                if (isOpen)
                {
                    gateMeshRenderers[i].material.SetColor(emissionColor, green * 5.0f);
                }
                else
                {
                    gateMeshRenderers[i].material.SetColor(emissionColor, red * 5.0f);
                }
            }
        }

        void ChangeCounterLights(bool isOn)
        {
            if (isOn)
            {
                lightMaterials[switchCount].SetColor(emissionColor, green * 5.0f);
            }
            else
            {
                lightMaterials[switchCount - 1].SetColor(emissionColor, red * 5.0f);
            }
        }

        void GateAnimation(bool isOpen)
        {
            if (!isEffect) return;

            gateLeft.DOLocalMoveX(isOpen ? 0.9f : 0, 0.3f);
            gateRight.DOLocalMoveX(isOpen ? -0.9f : 0, 0.3f);

            hologramMeshRenderer.material = isOpen ? openHoloMaterial : lockHoloMaterial;

            if (isOpen)
            {
                float alpha = 0.2f;
                hologramMeshRenderer.transform.DOShakeScale(0.3f);
                closeTween = DOTween.To(() => alpha, (a) => alpha = a, 0, 1.0f)
                    .SetDelay(0.3f)
                    .OnUpdate(() => { hologramMeshRenderer.material = isOpen ? openHoloMaterial : lockHoloMaterial; });
            }
        }

        void SetGate(bool isOpen)
        {
            gateLeft.transform.localPosition = new Vector3(isOpen ? 0.9f : 0, gateLeft.transform.localPosition.y, gateLeft.transform.localPosition.z);
            gateRight.transform.localPosition = new Vector3(isOpen ? -0.9f : 0, gateLeft.transform.localPosition.y, gateLeft.transform.localPosition.z);
            hologramMeshRenderer.material.SetFloat(alphaProperty, 0f);
        }

        void InstantiateCounterLights()
        {
            if (switchMaxCount > pooledLights.Length)
            {
                Debug.LogError("SwitchMaxCount is larger than the number of pooled lights.");
                return;
            }

            int numberOfGaps = switchMaxCount + 1;
            float spacing = setWidth / numberOfGaps;

            for (int i = 0; i < switchMaxCount; i++)
            {
                var lightObj = pooledLights[i];
                lightObj.SetActive(true);
                float xPosition = -setWidth / 2 + spacing * (i + 1);
                lightObj.transform.localPosition = new Vector3(xPosition, lightObj.transform.localPosition.y, lightObj.transform.localPosition.z);
                lightMaterials.Add(lightObj.GetComponent<Renderer>().material);
            }
        }
    }
}