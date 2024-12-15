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

        void Start()
        {
            playerController.IsDeath.Subscribe(isDeath =>
            {
                if (isDeath != PlayerController.deathType.isAlive)
                {
                    OnAttackHit(isDeath).Forget();
                }

            }).AddTo(this);
        }

        private readonly float shakeDuration = 1.5f;
        private readonly float shakeStrength = 0.7f;
        private readonly int shakeCount = 20;

        private readonly int hitStopDuration = 500;
        private readonly int hitStopDelay = 200;

        private async UniTaskVoid OnAttackHit(PlayerController.deathType type)
        {
            cameraPivot.DOShakePosition(shakeStrength, shakeDuration, shakeCount);

            effect.gameObject.SetActive(type == PlayerController.deathType.electro);
            poisonEffect.gameObject.SetActive(type == PlayerController.deathType.poison);

            if (type == PlayerController.deathType.electro) 
            {
                SoundManager.Instance.Play(SoundKey.ElectricShock, MixerType.SE);
            }else
            if (type == PlayerController.deathType.poison)
            {
                SoundManager.Instance.Play(SoundKey.Poison, MixerType.SE);
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