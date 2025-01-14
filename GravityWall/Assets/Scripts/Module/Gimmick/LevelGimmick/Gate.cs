using System.Collections.Generic;
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
        [Header("ゲートの開閉イベント")]
        [SerializeField] private UnityEvent gateOpenEvent;

        [SerializeField] private UnityEvent gateCloseEvent;

        [SerializeField] private GameObject gate;
        [SerializeField] private MeshRenderer[] gateMeshRenderers;
        [SerializeField] private Transform gateLeft, gateRight;
        [SerializeField] private GameObject[] pooledLights;
        [SerializeField] private int switchMaxCount;
        [SerializeField] private GimmickObject[] observedSwitches;
        [SerializeField] private float setWidth;
        [SerializeField] private float openGateWidth = 0.9f;
        [SerializeField] private float openGateDuration = 0.3f;
        [SerializeField, ReadOnly] private int usingCount = 0;
        [SerializeField] private Material lockHoloMaterial, openHoloMaterial;
        [SerializeField] private MeshRenderer hologramMeshRenderer;
        [SerializeField, ColorUsage(true, true)] private Color enableColor;
        [SerializeField, ColorUsage(true, true)] private Color disableColor;
        [SerializeField] private bool isEffect;

        private int switchCount = 0;
        private List<Material> lightMaterials = new List<Material>();
        private Tween closeTween;

        private static readonly int emissionColor = Shader.PropertyToID("_EmissionColor");
        private static readonly int alphaProperty = Shader.PropertyToID("_Alpha");

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
            // ライトの初期化
            InstantiateCounterLights();
            ChangeGateLight(false);

            foreach (GimmickObject gimmick in observedSwitches)
            {
                gimmick.IsEnabled.Skip(1).Subscribe(UpdateGateState).AddTo(this);
            }

            // 最初から有効の場合はゲートを開く
            if (isEnabled.Value)
            {
                SetGateNoEffect(true);
                gateOpenEvent.Invoke();
                gate.SetActive(false);
                ChangeGateLight(true);
            }
        }

        private void UpdateGateState(bool switchEnabled)
        {
            ChangeCounterLights(switchEnabled);

            // スイッチの入力に応じてカウントを増減
            switchCount += switchEnabled ? 1 : -1;

            // カウントが最大値に達したらゲートを開く
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
                SetGateNoEffect(true);
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
                SetGateNoEffect(false);
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

        private void ChangeGateLight(bool isOpen)
        {
            foreach (var gate in gateMeshRenderers)
            {
                gate.material.SetColor(emissionColor, isOpen ? enableColor : disableColor);
            }
        }

        private void ChangeCounterLights(bool isOn)
        {
            if (isOn)
            {
                lightMaterials[switchCount].SetColor(emissionColor, enableColor);
            }
            else
            {
                lightMaterials[switchCount - 1].SetColor(emissionColor, disableColor);
            }
        }

        private void GateAnimation(bool isOpen)
        {
            if (!isEffect)
            {
                return;
            }

            // ゲートの開閉アニメーション
            gateLeft.DOLocalMoveX(isOpen ? openGateWidth : 0, openGateDuration);
            gateRight.DOLocalMoveX(isOpen ? -openGateWidth : 0, openGateDuration);

            hologramMeshRenderer.material = isOpen ? openHoloMaterial : lockHoloMaterial;

            // 鍵穴の開放エフェクトの再生
            if (isOpen)
            {
                PlayKeyHoleEffect();
            }
        }

        private void PlayKeyHoleEffect()
        {
            float alpha = 0.2f;
            hologramMeshRenderer.transform.DOShakeScale(openGateDuration);
            hologramMeshRenderer.material = openHoloMaterial;

            closeTween = DOTween.To(() => alpha, (a) => alpha = a, 0, 1.0f)
                .SetDelay(openGateDuration)
                .OnUpdate(() => { hologramMeshRenderer.material.SetFloat(alphaProperty, alpha); });
        }

        private void SetGateNoEffect(bool isOpen)
        {
            float x = isOpen ? openGateWidth : 0;
            gateLeft.transform.localPosition = new Vector3(x, gateLeft.transform.localPosition.y, gateLeft.transform.localPosition.z);
            gateRight.transform.localPosition = new Vector3(-x, gateRight.transform.localPosition.y, gateRight.transform.localPosition.z);
            hologramMeshRenderer.material.SetFloat(alphaProperty, 0f);
        }

        private void InstantiateCounterLights()
        {
            if (switchMaxCount > pooledLights.Length)
            {
                Debug.LogError("SwitchMaxCount is larger than the number of pooled lights.");
                return;
            }

            // 設定された幅から間隔を計算する
            int numberOfGaps = switchMaxCount + 1;
            float spacing = setWidth / numberOfGaps;

            for (int i = 0; i < switchMaxCount; i++)
            {
                var lightObj = pooledLights[i];
                lightObj.SetActive(true);

                // 設定された幅で中央揃えで配置する
                float xPosition = -setWidth / 2 + spacing * (i + 1);

                // ローカル座標を変更
                Vector3 localPosition = lightObj.transform.localPosition;
                localPosition = new Vector3(xPosition, localPosition.y, localPosition.z);
                lightObj.transform.localPosition = localPosition;

                lightMaterials.Add(lightObj.GetComponent<Renderer>().material);
            }
        }
    }
}