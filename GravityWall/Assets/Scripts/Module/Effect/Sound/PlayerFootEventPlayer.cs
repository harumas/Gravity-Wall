using Core.Sound;
using CoreModule.Helper;
using CoreModule.Sound;
using PropertyGenerator.Generated;
using UnityEngine;

namespace Module.Effect.Sound
{
    public class PlayerFootEventPlayer : MonoBehaviour
    {
        [SerializeField] private float speedThreshold;
        [SerializeField] private MinMaxValue pitchRange;
        [SerializeField] private PlayerControllerWrapper anim;

        public void FootEvent()
        {
            if (anim.Speed > speedThreshold || anim.IsJumping)
            {
                float pitch = pitchRange.GetRandom();
                PlayContext context = new PlayContext(1f, pitch);

                SoundManager.Instance.Play(SoundKey.Foot, MixerType.SE, context);
            }
        }
    }
}