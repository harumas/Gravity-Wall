using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace Application.Sequence
{
    /// <summary>
    /// ゲームの状態を保持するクラス
    /// </summary>
    public class GameState
    {
        public enum State
        {
            Tutorial,
            NewGameSelected,
            StageSelect,
            Playing
        }

        private readonly ReactiveProperty<State> current = new();
        
        /// <summary>
        /// 現在のゲームの状態
        /// </summary>
        public ReadOnlyReactiveProperty<State> Current => current;

        /// <summary>
        /// ゲームの状態を設定します
        /// </summary>
        public void SetState(State state)
        {
            current.Value = state;
            Debug.Log($"GameState: {state}");
        }

        /// <summary>
        /// 指定のステートに切り替わるまで非同期で待機します
        /// </summary>
        public UniTask WaitUntilState(State state, CancellationToken cancellationToken)
        {
            return UniTask.WaitUntil(() => current.Value == state, cancellationToken: cancellationToken);
        }
    }
}