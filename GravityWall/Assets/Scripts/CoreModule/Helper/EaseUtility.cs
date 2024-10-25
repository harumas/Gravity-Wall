using DG.Tweening;
using DG.Tweening.Core.Easing;

namespace CoreModule.Helper
{
    public static class EaseUtility
    {
        public static float Evaluate(Ease easeType, float duration, float time)
        {
            if (easeType == Ease.Unset)
            {
                return time;
            }

            return EaseManager.Evaluate(easeType, null, time, duration, 1f, 1f);
        }
    }
}