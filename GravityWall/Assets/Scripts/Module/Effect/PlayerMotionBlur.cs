using Module.Player;
using R3;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Module.Effect
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