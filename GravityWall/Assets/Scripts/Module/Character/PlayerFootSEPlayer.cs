using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Character
{
    public class PlayerFootSEPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        public void FootSE()
        {
            Debug.Log("SE");
            audioSource.pitch = Random.Range(0.5f, 1.5f);
            audioSource.Play();
        }
    }
}