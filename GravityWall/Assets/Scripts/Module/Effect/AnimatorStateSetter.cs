using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Effect
{
    public class AnimatorStateSetter : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private readonly string paramaterName = "IsOpen";
        public void AnimatorSetBool(bool isActive)
        {
            animator.SetBool(paramaterName, isActive);
        }
    }
}