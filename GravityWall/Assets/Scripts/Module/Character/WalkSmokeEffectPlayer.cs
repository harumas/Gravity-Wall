using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

namespace Module.Character
{
    public class WalkSmokeEffectPlayer : MonoBehaviour
    {
        [SerializeField] private VisualEffect smokeEffect;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private float playInterval;

        private async void Start()
        {
            while (!destroyCancellationToken.IsCancellationRequested)
            {
                smokeEffect.Play();
                await UniTask.Delay(TimeSpan.FromSeconds(playInterval));
                await UniTask.WaitUntil(IsPlaySmoke);
            }
        }

        private bool IsPlaySmoke()
        {
            (Vector3 xv, Vector3 yv) = playerController.OnMove.CurrentValue;
            bool isMoving = xv != Vector3.zero || yv != Vector3.zero;
            return !playerController.IsJumping.CurrentValue && isMoving; 
        }
    }
}