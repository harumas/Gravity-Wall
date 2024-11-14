using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace Application.Sequence
{
    public class GameState
    {
        public enum State
        {
            StageSelect,
            Playing
        }

        private readonly ReactiveProperty<State> current = new();
        public ReadOnlyReactiveProperty<State> Current => current;

        public void SetState(State state)
        {
            current.Value = state;
            Debug.Log($"GameState: {state}");
        }

        public UniTask WaitUntilState(State state, CancellationToken cancellationToken)
        {
            return UniTask.WaitUntil(() => current.Value == state, cancellationToken: cancellationToken);
        }
    }
}