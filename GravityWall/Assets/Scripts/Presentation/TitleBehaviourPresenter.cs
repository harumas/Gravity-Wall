using Application;
using CoreModule.Save;
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
    public class TitleBehaviourPresenter : IStartable
    {
        private readonly ViewBehaviourNavigator navigator;
        private readonly TitleBehaviour titleBehaviour;
        private readonly ApplicationStopper applicationStopper;
        private readonly CursorLocker cursorLocker;
        private readonly PlayerController playerController;
        private readonly PlayerTargetSyncer playerTargetSyncer;
        private readonly SaveManager<SaveData> saveManager;
        private readonly SceneGroupTable sceneGroupTable;

        [Inject]
        public TitleBehaviourPresenter(
            ViewBehaviourNavigator navigator,
            TitleBehaviour titleBehaviour,
            ApplicationStopper applicationStopper,
            CursorLocker cursorLocker,
            PlayerController playerController,
            PlayerTargetSyncer playerTargetSyncer,
            SaveManager<SaveData> saveManager,
            SceneGroupTable sceneGroupTable)
        {
            this.navigator = navigator;
            this.titleBehaviour = titleBehaviour;
            this.applicationStopper = applicationStopper;
            this.cursorLocker = cursorLocker;
            this.playerController = playerController;
            this.playerTargetSyncer = playerTargetSyncer;
            this.saveManager = saveManager;
            this.sceneGroupTable = sceneGroupTable;
        }

        public void Start()
        {
            TitleView titleView = titleBehaviour.TitleView;

            //ゲーム終了は一度だけ入力を受け取る
            titleView.OnEndGameButtonPressed.Take(1).Subscribe(_ => applicationStopper.Quit());
            titleView.OnCreditButtonPressed.Subscribe(_ => navigator.ActivateBehaviour(ViewBehaviourState.Credit));
            titleView.OnNewGameButtonPressed.Subscribe(_ =>
            {
                // 仮実装
                SceneGroup sceneGroup = sceneGroupTable.SceneGroups[0];
                string mainSceneName = sceneGroup.GetMainScene();

                if (mainSceneName == SceneManager.GetActiveScene().name)
                {
                    navigator.DeactivateBehaviour(ViewBehaviourState.Title);
                    return;
                }

                saveManager.Reset();
                SceneManager.LoadScene(mainSceneName);
            });
            titleView.OnContinueGameButtonPressed.Subscribe(_ => navigator.DeactivateBehaviour(ViewBehaviourState.Title));

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
    }
}