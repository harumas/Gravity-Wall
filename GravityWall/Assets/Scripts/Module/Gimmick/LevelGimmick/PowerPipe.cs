using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using PropertyGenerator.Generated;
using UnityEngine;

namespace Module.Gimmick.LevelGimmick
{
    public class PowerPipe : MonoBehaviour
    {
        [SerializeField, Header("動力演出の変化時間")] private float fadeDuration;
        private PowerPipeShaderWrapper[] pipeRenderers;

        void Start()
        {
            // MeshRendererからシェーダーへの操作ラッパーを取得する
            pipeRenderers = GetComponentsInChildren<MeshRenderer>()
                .Select(renderer => new PowerPipeShaderWrapper(renderer.material))
                .ToArray();

            DoPowerEffect(false);
        }

        public void DoPowerEffect(bool isOn)
        {
            // 動力の変更演出
            foreach (var shaderWrapper in pipeRenderers)
            {
                float value = isOn ? 0.0f : 1.0f;
                DOTween.To(() => value, v => value = v, value, fadeDuration)
                    .OnUpdate(() =>
                    {
                        shaderWrapper.EmissionIntensity = value;
                    });
            }
        }
    }
}