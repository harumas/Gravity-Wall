using System.Collections;
using System.Collections.Generic;
using Module.Character;
using R3;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

public class DeathEffect : MonoBehaviour
{
    [SerializeField] private GameObject effect;
    [SerializeField] private AudioClip deathEffect;
    [SerializeField] private AudioSource audioSource;
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

    async UniTaskVoid OnAttackHit()
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
        audioSource.PlayOneShot(deathEffect);
        Invoke("OffEffect", 1.3f);
    }

    void OffEffect()
    {
        effect.gameObject.SetActive(false);
    }
}