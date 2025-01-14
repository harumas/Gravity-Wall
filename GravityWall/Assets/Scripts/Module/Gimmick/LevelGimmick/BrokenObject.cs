using CoreModule.Sound;
using DG.Tweening;
using UnityEngine;

namespace Module.Gimmick.LevelGimmick
{
    public class BrokenObject : MonoBehaviour
    {
        [SerializeField] private GameObject levels;
        [SerializeField] private GameObject effects;

        public Tween DoMove()
        {
            gameObject.SetActive(true);

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