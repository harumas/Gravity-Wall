using UnityEngine;
using CoreModule.Sound;
using Module.Gravity;
using Constants;

namespace Module.LevelGimmick
{
    public class TutorialBGMPlayer : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tag.Player))
            {
                if (WorldGravity.Instance.Gravity == Vector3.back)
                {
                    SoundManager.Instance.Play(Core.Sound.SoundKey.TutorialBGM1, Core.Sound.MixerType.BGM);
                }
            }
        }
    }
}