using System.Threading;
using Application;
using Application.Sequence;
using CoreModule.Save;
using Cysharp.Threading.Tasks;
using Module.Config;
using Module.InputModule;
using Module.Player;
using R3;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using View;

namespace Presentation
{
    public class TitleBehaviourPresenter : IStartable,IAsyncStartable
    {
        private readonly ViewBehaviourNavigator navigator;
        private readonly TitleBehaviour titleBehaviour;
        private readonly ApplicationStopper applicationStopper;
        private readonly CursorLocker cursorLocker;
        private readonly PlayerController playerController;
        private readonly PlayerTargetSyncer playerTargetSyncer;
        private readonly SceneGroupTable sceneGroupTable;
        private readonly GameState gameState;

        [Inject]
        public TitleBehaviourPresenter(
            ViewBehaviourNavigator navigator,
            TitleBehaviour titleBehaviour,
            ApplicationStopper applicationStopper,
            CursorLocker cursorLocker,
            PlayerController playerController,
            PlayerTargetSyncer playerTargetSyncer,
            SaveManager<SaveData> saveManager,
            SceneGroupTable sceneGroupTable,
            GameState gameState)
        {
            this.navigator = navigator;
            this.titleBehaviour = titleBehaviour;
            this.applicationStopper = applicationStopper;
            this.cursorLocker = cursorLocker;
            this.playerController = playerController;
            this.playerTargetSyncer = playerTargetSyncer;
            this.sceneGroupTable = sceneGroupTable;
            this.gameState = gameState;
        }

        public void Start()
        {
            TitleView titleView = titleBehaviour.TitleView;

            //ゲーム終了は一度だけ入力を受け取る
            titleView.OnEndGameButtonPressed.Take(1).Subscribe(_ => applicationStopper.Quit());
            titleView.OnCreditButtonPressed.Subscribe(_ => navigator.ActivateBehaviour(ViewBehaviourState.Credit));
            titleView.OnNewGameButtonPressed.Subscribe(_ =>
            {
                if (IsNewGame())
                {
                    navigator.DeactivateBehaviour(ViewBehaviourState.Title);
                }
                else
                {
                    navigator.ActivateBehaviour(ViewBehaviourState.ConfirmNewGame);
                }
            });

            if (IsNewGame())
            {
                titleView.DisableContinueButton();
            }
            else
            {
                titleView.OnContinueGameButtonPressed.Subscribe(_ => navigator.DeactivateBehaviour(ViewBehaviourState.Title));
            }

            //カーソルロックの変更に応じてプレイヤーの操作をロックする
            titleBehaviour.OnCursorLockChange.Subscribe(isLock =>
            {
                cursorLocker.SetCursorLock(isLock);

                if (isLock)
                {
                    playerTargetSyncer.Unlock();
                    playerController.Unlock();
                }
                else
                {
                    playerTargetSyncer.Lock();
                    playerController.Lock();
                }
            });
        }

        private bool IsNewGame()
        {
            SceneGroup sceneGroup = sceneGroupTable.SceneGroups[0];
            string mainSceneName = sceneGroup.GetMainScene();

            return mainSceneName == SceneManager.GetActiveScene().name;
        }

        public async UniTask StartAsync(CancellationToken cancellation = new CancellationToken())
        {
            await UniTask.Yield(cancellation);
            
            if (gameState.Current.CurrentValue == GameState.State.NewGameSelected)
            {
                navigator.DeactivateBehaviour(ViewBehaviourState.Title);
            }
        }
    }
}