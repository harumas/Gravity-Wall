using UnityEngine;

namespace Module.Gimmick
{
    public abstract class AbstractSwitch : MonoBehaviour
    {
        public abstract bool isOn { get; protected set; }
        public abstract void OnSwitch(bool isOn);
    }
}