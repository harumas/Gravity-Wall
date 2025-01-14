using UnityEngine;
using CoreModule.Sound;

namespace Module.Effect.Sound
{
    public class PlugSoundPlayer : MonoBehaviour
    {
        public void PlayPlugSound(int plugOn)
        {
            if (plugOn == 1) 
            {
                SoundManager.Instance.Play(Core.Sound.SoundKey.PlugOn, Core.Sound.MixerType.SE);
            }
            else
            {
                SoundManager.Instance.Play(Core.Sound.SoundKey.PlugOff, Core.Sound.MixerType.SE);
            }
        }
    }
}