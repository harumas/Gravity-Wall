using UnityEngine;
using R3;

namespace Module.Character
{
    public class PlayerRotateSEPlayer : MonoBehaviour
    {
        [SerializeField] private float playInterval = 0.3f;
        [SerializeField] private AudioClip audioClip;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private PlayerController playerController;
        private bool isRotating = true;
        
        private float lastPlayTime;
        
        void Start()
        {
            // 回転のイベント登録
            playerController.IsRotating.Subscribe(isRotating =>
            {
                if (lastPlayTime + playInterval > Time.time)
                {
                    return;
                }
                
                if (this.isRotating == false)
                {
                    audioSource.clip = audioClip;
                    audioSource.Play();
                    lastPlayTime = Time.time;
                }

                this.isRotating = isRotating;
            }).AddTo(this);
        }
    }
}