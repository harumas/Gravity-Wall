using System;
using Application.Sequence;
using CoreModule.Save;
using Cysharp.Threading.Tasks;
using Module.Config;
using R3;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using View;
using View.Behaviour;
using View.View;

namespace Presentation
{
    public class ConfirmNewGamePresenter : IStartable
    {
        private readonly ConfirmNewGameBehaviour confirmNewGameBehaviour;
        private readonly SaveManager<SaveData> saveManager;
        private readonly SceneGroupTable sceneGroupTable;
        private readonly GameState gameState;
        private readonly ViewBehaviourNavigator navigator;
        
        private readonly float confirmDelay = 0.5f;

        [Inject]
        public ConfirmNewGamePresenter(
            ConfirmNewGameBehaviour confirmNewGameBehaviour,
            SaveManager<SaveData> saveManager,
            SceneGroupTable sceneGroupTable,
            GameState gameState,
            ViewBehaviourNavigator navigator)
        {
            this.confirmNewGameBehaviour = confirmNewGameBehaviour;
            this.saveManager = saveManager;
            this.sceneGroupTable = sceneGroupTable;
            this.gameState = gameState;
            this.navigator = navigator;
        }

        public void Start()
        {
            ConfirmNewGameView view = confirmNewGameBehaviour.ConfirmNewGameView;
            view.OnConfirmButtonPressed.Subscribe(OnConfirmButtonPressed).AddTo(view);
            view.OnCancelButtonPressed.Subscribe(_ => navigator.DeactivateBehaviour(ViewBehaviourState.ConfirmNewGame)).AddTo(view);
        }

        private async void OnConfirmButtonPressed(Unit _)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(confirmDelay));

            gameState.SetState(GameState.State.NewGameSelected);
            saveManager.Reset();
            SceneManager.LoadScene(sceneGroupTable.SceneGroups[0].GetMainScene());
        }
    }
}