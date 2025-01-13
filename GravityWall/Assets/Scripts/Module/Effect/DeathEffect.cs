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

        [SerializeField] private float shakeDuration;
        [SerializeField] private float shakeStrength;
        [SerializeField] private int shakeCount;
        [SerializeField] private int hitStopDuration;
        [SerializeField] private int hitStopDelay;

        private void Start()
        {
            playerController.IsDeath.Subscribe(isDeath =>
            {
                // 死亡状態になったらエフェクトを再生
                if (isDeath != PlayerController.DeathType.None)
                {
                    OnAttackHit(isDeath).Forget();
                }
            }).AddTo(this);
        }

        private async UniTaskVoid OnAttackHit(PlayerController.DeathType type)
        {
            // カメラを揺らす
            cameraPivot.DOShakePosition(shakeStrength, shakeDuration, shakeCount);

            // SE & エフェクト再生
            PlayEffect(type);

            //ヒットストップ
            await UniTask.Delay(hitStopDelay);
            anim.speed = 0f;
            await UniTask.Delay(hitStopDuration);
            anim.speed = 1f;

            effect.gameObject.SetActive(false);
            poisonEffect.gameObject.SetActive(false);
        }

        private void PlayEffect(PlayerController.DeathType type)
        {
            switch (type)
            {
                case PlayerController.DeathType.Electric:
                    effect.gameObject.SetActive(true);
                    SoundManager.Instance.Play(SoundKey.ElectricShock, MixerType.SE);
                    break;

                case PlayerController.DeathType.Poison:
                    poisonEffect.gameObject.SetActive(true);
                    SoundManager.Instance.Play(SoundKey.Poison, MixerType.SE);
                    break;
            }
        }
    }
}