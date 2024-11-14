using System.Threading;
using Application.Sequence;
using Cysharp.Threading.Tasks;
using Module.Gimmick;
using Module.Gimmick.LevelGimmick;
using UnityEngine;
using VContainer.Unity;

namespace Presentation
{
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