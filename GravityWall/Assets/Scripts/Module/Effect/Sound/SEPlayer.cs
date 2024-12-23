using UnityEngine;

namespace Module.Effect.Sound
{
    public class SEPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip audioClip;
        private AudioSource audioSource;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void Play()
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}
