using System.Collections.Generic;
using R3;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Module.Gimmick.LevelGimmick
{
    public class HugeGate : Gate
    {
        [SerializeField] private Animator anim;

        public void OnOpenAnimationEvent(bool isOpen)
        {
            anim.SetBool("IsOpen", isOpen);
        }
    }
}