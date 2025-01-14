using Core.Sound;
using CoreModule.Sound;
using UnityEngine;

namespace Module.Effect.Sound
{
    public class PressurePlateSoundPlayer : MonoBehaviour
    {
        public void Play()
        {
            SoundManager.Instance.Play(SoundKey.Switch, MixerType.SE);
        }
    }
}