using UnityEngine;
using R3;

namespace Module.Character
{
    public class PlayerRotateSEPlayer : MonoBehaviour
    {
        [SerializeField] private float volume = 0.8f;
        [SerializeField] private AudioClip audioClip;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private PlayerController playerController;
        private bool isRotating = true;
        // Start is called before the first frame update
        void Start()
        {
            // 回転のイベント登録
            playerController.IsRotating.Subscribe(isRotating =>
            {
                if (this.isRotating == false)
                {
                    audioSource.volume = volume;
                    audioSource.clip = audioClip;
                    audioSource.Play();
                }

                this.isRotating = isRotating;
            }).AddTo(this);
        }
    }
}