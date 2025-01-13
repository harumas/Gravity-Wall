using Core.Sound;
using CoreModule.Helper;
using CoreModule.Sound;
using PropertyGenerator.Generated;
using UnityEngine;

namespace Module.Effect.Sound
{
    /// <summary>
    /// プレイヤーの足音を再生するクラス
    /// </summary>
    public class PlayerFootEventPlayer : MonoBehaviour
    {
        [SerializeField] private float speedThreshold;
        [SerializeField] private MinMaxValue pitchRange;
        [SerializeField] private PlayerControllerWrapper anim;

        public void FootEvent()
        {
            // 速度が一定以上の場合は足音を再生
            if (anim.Speed > speedThreshold || anim.IsJumping)
            {
                // ピッチをランダムにして足音を変化させる
                float pitch = pitchRange.GetRandom();
                PlayContext context = new PlayContext(1f, pitch);

                SoundManager.Instance.Play(SoundKey.Foot, MixerType.SE, context);
            }
        }
    }
}