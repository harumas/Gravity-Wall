using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessing
{
    [Serializable, VolumeComponentMenu("Custom/Screen Space Reflection")]
    public class ScreenSpaceReflection : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter intensity = new ClampedFloatParameter(value: 0f, min: 0, max: 1, overrideState: true);

        public bool IsActive() => intensity.value > 0;

        public bool IsTileCompatible() => true;
    }
}