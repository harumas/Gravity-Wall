using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace CoreModule.Helper
{
    public static class DoTweenHelper
    {
        public static TweenerCore<Vector3, Vector3, VectorOptions> DoMove(this Rigidbody rigidbody, Vector3 position, float duration)
        {
            return DOTween.To(() => rigidbody.position, v => rigidbody.position = v, position, duration);
        }
    }
}