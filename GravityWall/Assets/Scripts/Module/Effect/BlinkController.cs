using System;
using CoreModule.Helper;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Module.Effect
{
    /// <summary>
    /// プレイヤーの瞬きを制御するクラス
    /// </summary>
    public class BlinkController : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer faceMeshRenderer;
        [SerializeField] private MinMaxValue blinkTime;

        private static readonly int rEyeBlink = Shader.PropertyToID("_REyeBlink");
        private static readonly int lEyeBlink = Shader.PropertyToID("_LEyeBlink");

        private readonly int faceMaterialEyesIndex = 1;

        private async void Start()
        {
            while (!destroyCancellationToken.IsCancellationRequested)
            {
                DOBlink();

                // 瞬きの間隔をランダムに設定
                float waitTime = blinkTime.GetRandom();
                await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: destroyCancellationToken);
            }
        }

        private void DOBlink()
        {
            float scale = 0;
            
            // 目を小さくしてから元に戻す
            DOTween.To(() => scale, (v) => scale = v, 3, 0.15f)
                .SetLoops(2, LoopType.Yoyo)
                .OnUpdate(() =>
                {
                    faceMeshRenderer.materials[faceMaterialEyesIndex].SetFloat(rEyeBlink, scale);
                    faceMeshRenderer.materials[faceMaterialEyesIndex].SetFloat(lEyeBlink, scale);
                });
        }
    }
}