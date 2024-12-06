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
                    OnDeathEffect();
                    OnAttackHit().Forget();
                }

            }).AddTo(this);
        }

        private async UniTaskVoid OnAttackHit()
        {
            cameraPivot.DOShakePosition(0.7f, 1.5f, 20);
            await UniTask.Delay(200);
            anim.speed = 0f;
            await UniTask.Delay(500);
            anim.speed = 1f;
        }

        public void OnDeathEffect()
        {
            effect.gameObject.SetActive(true);
            SoundManager.Instance.Play(SoundKey.ElectricShock, MixerType.SE);
            Invoke("OffEffect", 1.3f);
        }

        private void OffEffect()
        {
            effect.gameObject.SetActive(false);
        }
    }
}