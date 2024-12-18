using Core.Sound;
using CoreModule.Sound;
using R3;
using UnityEngine;
using Module.Player;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace Module.Effect
{
    public class DeathEffect : MonoBehaviour
    {
        [SerializeField] private GameObject effect, poisonEffect;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Animator anim;
        
        [SerializeField] private float shakeDuration = 1.5f;
        [SerializeField] private float shakeStrength = 0.7f;
        [SerializeField] private int shakeCount = 20;
        [SerializeField] private int hitStopDuration = 500;
        [SerializeField] private int hitStopDelay = 200;

        private void Start()
        {
            playerController.IsDeath.Subscribe(isDeath =>
            {
                if (isDeath != PlayerController.DeathType.IsAlive)
                {
                    OnAttackHit(isDeath).Forget();
                }
            }).AddTo(this);
        }

        private async UniTaskVoid OnAttackHit(PlayerController.DeathType type)
        {
            cameraPivot.DOShakePosition(shakeStrength, shakeDuration, shakeCount);

            effect.gameObject.SetActive(type == PlayerController.DeathType.Electric);
            poisonEffect.gameObject.SetActive(type == PlayerController.DeathType.Poison);

            switch (type)
            {
                case PlayerController.DeathType.Electric:
                    SoundManager.Instance.Play(SoundKey.ElectricShock, MixerType.SE);
                    break;
                case PlayerController.DeathType.Poison:
                    SoundManager.Instance.Play(SoundKey.Poison, MixerType.SE);
                    break;
            }

            await UniTask.Delay(hitStopDelay);

            //ヒットストップ
            anim.speed = 0f;
            await UniTask.Delay(hitStopDuration);
            anim.speed = 1f;

            effect.gameObject.SetActive(false);
            poisonEffect.gameObject.SetActive(false);
        }
    }
}