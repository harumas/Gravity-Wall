using System;
using R3;
using UnityEngine;

namespace Module.Gimmick
{
    public class JustOnceGimmick : MonoBehaviour
    {
        [SerializeField] private GimmickObject targetGimmick;

        private void Awake()
        {
            targetGimmick.IsEnabled.Skip(1).Subscribe(isEnabled =>
            {
                if (!isEnabled)
                {
                    gameObject.SetActive(false);
                }
            }).AddTo(this);
        }
    }
}
