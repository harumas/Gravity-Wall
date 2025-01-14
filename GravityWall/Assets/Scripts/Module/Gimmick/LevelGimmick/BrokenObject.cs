using CoreModule.Sound;
using DG.Tweening;
using UnityEngine;

namespace Module.Gimmick.LevelGimmick
{
    /// <summary>
    /// エンディングの破壊オブジェクトの移動コンポーネント
    /// </summary>
    public class BrokenObject : MonoBehaviour
    {
        [SerializeField] private GameObject levels;
        [SerializeField] private GameObject effects;

        public Tween DoMove()
        {
            gameObject.SetActive(true);
            levels.gameObject.SetActive(true);

            // 初期位置まで移動させてからエフェクトを再生
            return levels.transform
                .DOLocalMove(Vector3.zero, 0.5f)
                .OnComplete(() =>
                {
                    effects.SetActive(true);
                    SoundManager.Instance.Play(Core.Sound.SoundKey.Bomb, Core.Sound.MixerType.SE);
                });
        }
    }
}