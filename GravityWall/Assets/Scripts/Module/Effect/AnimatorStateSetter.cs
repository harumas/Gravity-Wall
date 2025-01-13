using UnityEngine;

namespace Module.Effect
{
    public class AnimatorStateSetter : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string parameterName = "IsOpen";

        public void AnimatorSetBool(bool isActive)
        {
            animator.SetBool(parameterName, isActive);
        }
    }
}