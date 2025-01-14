using Module.Gimmick.LevelGimmick;
using R3;
using UnityEngine;

namespace Module.Gimmick.SystemGimmick
{
    public class JustOnceStartGate : MonoBehaviour
    {
        [SerializeField] private Gate targetGate;
        [SerializeField] private StartRoom startRoom;

        private void Awake()
        {
            targetGate.IsEnabled.Skip(1).Subscribe(isEnabled =>
            {
                if (!startRoom.IsPlayerEnter && !isEnabled)
                {
                    gameObject.SetActive(false);
                }
            }).AddTo(this);
        }

        public void Reset()
        {
            gameObject.SetActive(true);
            targetGate.gameObject.SetActive(true);
            targetGate.Reset();
        }
    }
}