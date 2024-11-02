using System.Collections;
using System.Collections.Generic;
using Module.Character;
using R3;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    [SerializeField] private GameObject effect;
    [SerializeField] private AudioClip deathEffect;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private PlayerController playerController;
    
    void Start()
    {
        playerController.IsDeath.Subscribe(isDeath =>
        {
            if (isDeath)
            {
                OnDeathEffect();
            }
        }).AddTo(this);
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