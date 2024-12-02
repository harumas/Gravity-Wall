using System.Threading;
using Application.Spawn;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Application.Sequence
{
    /// <summary>
    /// レベル内のゲーム進行を行うクラス
    /// </summary>
    public class InLevelSequencer : IAsyncStartable
    {
        private readonly RespawnManager respawnManager;
        private readonly GameState gameState;

        public InLevelSequencer(RespawnManager respawnManager,GameState gameState)
        {
            this.gameState = gameState;
            this.respawnManager = respawnManager;
        }

        public async UniTask StartAsync(CancellationToken cancellation = new CancellationToken())
        {
            gameState.SetState(GameState.State.Playing);
            
            await gameState.WaitUntilState(GameState.State.StageSelect, cancellation);
        }
    }
}