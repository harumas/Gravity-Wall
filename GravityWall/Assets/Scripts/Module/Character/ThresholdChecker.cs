using UnityEngine;

namespace Module.Character
{
    public class ThresholdChecker
    {
        private readonly float threshold;
        private readonly float duration;

        private bool enabled;
        private float startTime;
        private bool isCounting;

        public ThresholdChecker(float threshold, float duration)
        {
            this.threshold = threshold;
            this.duration = duration;
        }

        public bool IsUnderThreshold(float value)
        {
            if (!enabled)
            {
                return false;
            }

            bool isUnderThreshold = value < threshold;

            if (isCounting)
            {
                bool isOverTime = Time.time - startTime > duration;
                if (!isUnderThreshold || isOverTime)
                {
                    Disable();
                }

                return isUnderThreshold;
            }

            if (isUnderThreshold)
            {
                startTime = Time.time;
                isCounting = true;
                return true;
            }

            return false;
        }

        public void Enable()
        {
            enabled = true;
            isCounting = false;
        }

        public void Disable()
        {
            enabled = false;
            isCounting = false;
        }
    }
}