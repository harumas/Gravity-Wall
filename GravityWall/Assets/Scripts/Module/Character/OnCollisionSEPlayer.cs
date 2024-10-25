using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollisionSEPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private void OnCollisionEnter(Collision collision)
    {
        audioSource.Play();
    }
}
