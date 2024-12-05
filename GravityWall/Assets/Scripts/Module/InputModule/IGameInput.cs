using R3;
using UnityEngine;

namespace Module.InputModule
{
    public interface IGameInput
    {
        ReadOnlyReactiveProperty<Vector2> LookDelta { get; }
        ReadOnlyReactiveProperty<Vector2> Move { get; }
        Observable<int> CameraRotate { get; }
        Observable<bool> Jump { get; }
    }
}