using Application.Sequence;
using CoreModule.Save;
using Module.Config;
using R3;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using View;
using View.Behaviour;

namespace Presentation
{
    public class ConfirmNewGamePresenter : IStartable
    {
        private readonly ConfirmNewGameBehaviour confirmNewGameBehaviour;
        private readonly SaveManager<SaveData> saveManager;
        private readonly SceneGroupTable sceneGroupTable;
        private readonly GameState gameState;

        [Inject]
        public ConfirmNewGamePresenter(
            ConfirmNewGameBehaviour confirmNewGameBehaviour,
            SaveManager<SaveData> saveManager,
            SceneGroupTable sceneGroupTable,
            GameState gameState)
        {
            this.confirmNewGameBehaviour = confirmNewGameBehaviour;
            this.saveManager = saveManager;
            this.sceneGroupTable = sceneGroupTable;
            this.gameState = gameState;
        }

        public void Start()
        {
            ConfirmNewGameView view = confirmNewGameBehaviour.ConfirmNewGameView;
            view.OnConfirmButtonPressed.Subscribe(_ =>
            {
                gameState.SetState(GameState.State.NewGameSelected);
                saveManager.Reset();
                SceneManager.LoadScene(sceneGroupTable.SceneGroups[0].GetMainScene());
            }).AddTo(view);
            view.OnCancelButtonPressed.Subscribe(_ => confirmNewGameBehaviour.Deactivate(ViewBehaviourState.Title)).AddTo(view);
        }
    }
}