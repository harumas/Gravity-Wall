using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    [SerializeField] private GameObject effect;
    [SerializeField] private AudioClip deathEffect;
    [SerializeField] private AudioSource audioSource;

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