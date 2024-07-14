using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PostProcessing
{
    [Serializable, VolumeComponentMenu("Custom/Color Transition")]
    public class ColorTransition : VolumeComponent, IPostProcessComponent
    {
        public ClampedFloatParameter intensity = new ClampedFloatParameter(value: 0f, min: 0, max: 1, overrideState: true);
        public FloatParameter speed = new FloatParameter(1, true);
        public ClampedFloatParameter saturation = new ClampedFloatParameter(value: 1, min: 0, max: 1, overrideState: true);
        public ClampedFloatParameter brightness = new ClampedFloatParameter(value: 1, min: 0, max: 1, overrideState: true);

        public bool IsActive() => intensity.value > 0;

        public bool IsTileCompatible() => true;
    }
}