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

        public bool IsOverThreshold(float value)
        {
            if (!enabled)
            {
                return false;
            }
            
            bool isOverThreshold = value > threshold;

            if (isCounting)
            {
                bool isOverTime = Time.time - startTime > duration;
                if (!isOverThreshold || isOverTime)
                {
                    Disable();
                    isCounting = false;
                }

                return isOverThreshold;
            }

            if (isOverThreshold)
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
        }
        
        public void Disable()
        {
            enabled = false;
        }
    }
}