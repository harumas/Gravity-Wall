using DG.Tweening;
using DG.Tweening.Core.Easing;

namespace CoreModule.Helper
{
    public static class EaseUtility
    {
        public static float Evaluate(Ease easeType, float angle, float step)
        {
            if (easeType == Ease.Unset)
            {
                return step;
            }

            return EaseManager.Evaluate(easeType, null, step, angle, 1f, 1f);
        }
    }
}