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
        [SerializeField] private GameObject effect;
        [SerializeField] private PlayerController playerController;
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Animator anim;

        void Start()
        {
            playerController.IsDeath.Subscribe(isDeath =>
            {
                if (isDeath)
                {
                    OnAttackHit().Forget();
                }

            }).AddTo(this);
        }

        private readonly float shakeDuration = 1.5f;
        private readonly float shakeStrength = 0.7f;
        private readonly int shakeCount = 20;

        private readonly int hitStopDuration = 500;
        private readonly int hitStopDelay = 200;

        private async UniTaskVoid OnAttackHit()
        {
            cameraPivot.DOShakePosition(shakeStrength, shakeDuration, shakeCount);

            effect.gameObject.SetActive(true);
            SoundManager.Instance.Play(SoundKey.ElectricShock, MixerType.SE);

            await UniTask.Delay(hitStopDelay);

            //ヒットストップ
            anim.speed = 0f;
            await UniTask.Delay(hitStopDuration);
            anim.speed = 1f;


            effect.gameObject.SetActive(false);
        }
    }
}