using Application.Sequence;
using UnityEngine;
using VContainer.Unity;

namespace Presentation
{
    public class GameClearPresenter : IStartable
    {
        private readonly GameState gameState;

        public GameClearPresenter(GameState gameState)
        {
            this.gameState = gameState;
        }

        public void Start()
        {
            var clearPoints = Object.FindObjectsByType<GameClearPoint>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (GameClearPoint clearPoint in clearPoints)
            {
                clearPoint.OnClear += () =>
                {
                    gameState.SetState(GameState.State.StageSelect);
                };
            }
        }
    }
}