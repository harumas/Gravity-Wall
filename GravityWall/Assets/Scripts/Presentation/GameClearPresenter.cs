using Application.Sequence;
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
            
        }
    }
}