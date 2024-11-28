using UnityEngine;

namespace Module.Gimmick.LevelGimmick
{
    public class HugeGateSEPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip open, unlock, steam;
        public void UnlockSEPlay()
        {
            audioSource.PlayOneShot(unlock);
        }

        public void OpenSEPlay()
        {
            audioSource.PlayOneShot(open);
        }

        public void SteamSEPlay()
        {
            audioSource.PlayOneShot(steam);
        }
    }
}