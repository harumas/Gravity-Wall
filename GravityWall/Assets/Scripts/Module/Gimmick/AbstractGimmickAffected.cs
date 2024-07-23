using UnityEngine;

namespace Module.Gimmick
{
    public abstract class AbstractGimmickAffected : MonoBehaviour
    {
        public abstract void Affect(AbstractSwitch switchObject);
    }
}