using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace View
{
    public class LoadingView : MonoBehaviour
    {
        [SerializeField] private Transform circleMask;
        public Transform CircleMask => circleMask;
    }
}