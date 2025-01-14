using Module.Gimmick.LevelGimmick;
using R3;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    public class JustOnceGate : MonoBehaviour
    {
        [SerializeField] private Gate targetGate;
        [SerializeField] private LevelActivator levelActivator;

        private void Awake()
        {
            targetGate.IsEnabled.Skip(1).Subscribe(isEnabled =>
            {
                if (levelActivator.IsPlayerEnter && targetGate.UsingCount == 1 && !isEnabled)
                {
                    gameObject.SetActive(false);
                }
            }).AddTo(this);
        }
    }
}
