using UnityEngine;

namespace Module.Player
{
    public class ThresholdChecker
    {
        private readonly float threshold;
        private readonly float duration;

        private bool enabled;
        private float startTime;
        private bool isCounting;
        private float previousValue;

        public ThresholdChecker(float threshold, float duration)
        {
            this.threshold = threshold;
            this.duration = duration;
        }
        
        public bool IsOverThreshold(float value)
        {
            return value > threshold;
        }

        public bool TryThresholdCount(float value, bool diffCheck)
        {
            if (diffCheck && previousValue != value)
            {
                Enable();
                startTime = Time.time;
                previousValue = value;
            }

            if (!enabled)
            {
                return false;
            }

            bool isUnderThreshold = value > threshold;

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
                previousValue = value;
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