using Cysharp.Threading.Tasks;
using UnityEngine;

namespace View
{
    public abstract class ViewBehaviour : MonoBehaviour
    {
        public abstract UniTask OnPreActive();
        public abstract UniTask OnActive();
        public abstract UniTask OnDeactive();

    }
}