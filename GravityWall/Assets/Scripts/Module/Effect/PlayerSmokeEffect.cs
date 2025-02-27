﻿using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Module.Player;
using PropertyGenerator.Generated;
using R3;
using UnityEngine;

namespace Module.Effect
{
    public class PlayerSmokeEffect : MonoBehaviour
    {
        [SerializeField] private SmokeDashVer1Wrapper walkEffect;
        [SerializeField] private SmokeJumpWrapper jumpEffect;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private float playInterval;
        [SerializeField] private AnimationCurve spawnRateCurve;
        [SerializeField] private AnimationCurve spawnSizeCurve;
        [SerializeField] private AnimationCurve velocityCurve;

        private void Start()
        {
            playerController.ControlEvent.IsExternalForce.Subscribe(isJumping =>
            {
                if (isJumping)
                {
                    PlayJumpEffect();
                }
            }).AddTo(this);

            RunSmokeEffectLoop(destroyCancellationToken).Forget();
        }

        private void PlayJumpEffect()
        {
            jumpEffect.Effect.Play();
        }

        private async UniTaskVoid RunSmokeEffectLoop(CancellationToken cancellationToken)
        {
            float movedTime = 0f;

            while (!cancellationToken.IsCancellationRequested)
            {
                // 移動中でない場合は待機
                await UniTask.WaitUntil(() =>
                {
                    bool isPlaySmoke = CanPlaySmoke();

                    if (!isPlaySmoke)
                    {
                        movedTime = Time.time;
                    }

                    return isPlaySmoke;
                }, cancellationToken: cancellationToken);

                // 移動時間を取得
                float time = Time.time - movedTime;

                // 移動時間を元にエフェクトを調整
                walkEffect.SpawnRate = spawnRateCurve.Evaluate(time);
                walkEffect.SmokeSize = spawnSizeCurve.Evaluate(time);
                walkEffect.SmokeVelocity = new Vector3(0f, 0f, velocityCurve.Evaluate(time));
                
                walkEffect.Effect.Play();

                await UniTask.Delay(TimeSpan.FromSeconds(playInterval), cancellationToken: cancellationToken);
            }
        }

        private bool CanPlaySmoke()
        {
            // 着地していて移動中の場合
            (Vector3 vx, Vector3 vy) = GetMovement();
            bool isMoving = vx != Vector3.zero || vy != Vector3.zero;
            
            return !playerController.ControlEvent.IsExternalForce.CurrentValue && isMoving;
        }

        private (Vector3 xv, Vector3 yv) GetMovement()
        {
            return playerController.ControlEvent.MoveVelocity.CurrentValue;
        }
    }
}