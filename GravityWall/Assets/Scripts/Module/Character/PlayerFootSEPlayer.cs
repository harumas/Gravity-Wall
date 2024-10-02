using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Character
{
    public class PlayerFootSEPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Animator anim;

        public void FootSE()
        {
            if (anim.GetFloat("Speed") > 0.1f)
            {
                audioSource.pitch = Random.Range(0.7f, 1.3f);
                audioSource.Play();
            }
        }
    }
}