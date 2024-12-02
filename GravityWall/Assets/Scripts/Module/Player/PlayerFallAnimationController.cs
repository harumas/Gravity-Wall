using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Character
{
    public class PlayerFallAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(""))
            {
                animator.SetInteger("FallIndex", 1);
            }
        }
    }
}