using R3;
using UnityEngine;

namespace Module.InputModule
{
    public interface IGameInput
    {
        Vector2 MouseDelta { get; }
        ReadOnlyReactiveProperty<Vector2> Move { get; }
        Observable<Unit> Jump { get; }
    }
}