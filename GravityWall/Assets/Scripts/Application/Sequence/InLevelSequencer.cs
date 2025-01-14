using System.Threading;
using CoreModule.Save;
using Cysharp.Threading.Tasks;
using Module.Config;
using VContainer.Unity;

namespace Application.Sequence
{
    /// <summary>
    /// レベル内のゲーム進行を行うクラス
    /// </summary>
    public class InLevelSequencer : IAsyncStartable
    {
        private readonly GameState gameState;
        private readonly SaveManager<SaveData> saveManager;

        public InLevelSequencer(GameState gameState, SaveManager<SaveData> saveManager)
        {
            this.gameState = gameState;
            this.saveManager = saveManager;
        }

        public async UniTask StartAsync(CancellationToken cancellation = new CancellationToken())
        {
            bool isTutorial = !saveManager.Data.ClearedStageList[0];
            
            if (isTutorial)
            {
                gameState.SetState(GameState.State.Tutorial);
            }
            else
            {
                gameState.SetState(GameState.State.Playing);
            }


            await gameState.WaitUntilState(GameState.State.StageSelect, cancellation);
        }
    }
}