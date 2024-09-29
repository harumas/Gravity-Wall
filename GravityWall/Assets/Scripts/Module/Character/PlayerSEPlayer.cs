using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PropertyGenerator.Generated;
using R3;
namespace Module.Character
{
    public class PlayerSEPlayer : MonoBehaviour
    {
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
                    audioSource.Play();
                }

                this.isRotating = isRotating;
            }).AddTo(this);
        }
    }
}