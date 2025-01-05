using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module.Player
{
    public class CameraShaker : MonoBehaviour
    {
        [SerializeField] private Transform pivot;
        private Tweener tweener;

        public void ShakeCamera(float time,float strength)
        {
            tweener?.Kill();
            tweener = pivot.DOShakePosition(time,strength,20);
        }
    }
}