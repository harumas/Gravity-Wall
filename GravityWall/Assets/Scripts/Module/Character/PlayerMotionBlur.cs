using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using R3;

namespace Module.Character
{
    public class PlayerMotionBlur : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        MotionBlur blur;
        private Volume volume;
        private void Start()
        {
            volume = GameObject.Find("Global Volume").GetComponent<Volume>();
            // 回転のイベント登録
            playerController.IsRotating.Subscribe(isRotating =>
            {
                if (volume.profile.TryGet<MotionBlur>(out blur))
                {
                    blur.active = isRotating;
                }
            }).AddTo(this);
        }
    }
}