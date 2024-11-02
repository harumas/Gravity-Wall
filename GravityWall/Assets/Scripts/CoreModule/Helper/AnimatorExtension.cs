using System.Linq;
using UnityEngine;

namespace CoreModule.Helper
{
    public static class AnimatorExtension
    {
        public static float GetAnimationClipLength(this Animator animator, string clipName)
        {
            AnimationClip clip = animator.runtimeAnimatorController.animationClips.FirstOrDefault(clip => clip.name == clipName);
            return clip == null ? -1 : clip.length;
        }
    }
}