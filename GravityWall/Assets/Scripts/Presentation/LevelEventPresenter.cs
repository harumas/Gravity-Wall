using System.Data;
using Application.Spawn;
using CoreModule.Helper;
using Cysharp.Threading.Tasks;
using Module.Gimmick;
using Module.Gimmick.LevelGimmick;
using Module.Gimmick.SystemGimmick;
using Module.Player;
using VContainer;
using VContainer.Unity;
using View;
using static Module.Player.PlayerController;

namespace Presentation
{
    public class LevelEventPresenter : IInitializable
    {
        private readonly RespawnManager respawnManager;
        private readonly PlayerController playerController;
        private readonly ViewBehaviourNavigator behaviourNavigator;
        private RespawnContext respawnDataOnDeath;

        [Inject]
        public LevelEventPresenter(RespawnManager respawnManager,
            ViewBehaviourNavigator behaviourNavigator,
            PlayerController playerController,
            ReusableComponents<SavePoint> savePointComponents,
            ReusableComponents<DeathFloor> deathFloorComponents)
        {
            this.respawnManager = respawnManager;
            this.behaviourNavigator = behaviourNavigator;
            this.playerController = playerController;

            SubscribeComponents(savePointComponents, deathFloorComponents);
        }

        private void OnSave(RespawnContext respawnContext)
        {
            respawnDataOnDeath = respawnContext;
        }

        private void SubscribeComponents(
            ReusableComponents<SavePoint> savePointComponents,
            ReusableComponents<DeathFloor> deathFloorComponents)
        {
            var savePoints = savePointComponents.GetComponents();
            var deathFloors = deathFloorComponents.GetComponents();

            //セーブポイントのイベント登録
            foreach (SavePoint savePoint in savePoints)
            {
                savePoint.OnEnterPoint += OnSave;

                // 既にセーブ処理が実行されていたら、そのセーブ情報でセーブを行う
                if (savePoint.IsSaved)
                {
                    OnSave(savePoint.LatestContext);    
                }
            }

            //死亡床のイベント登録
            foreach (DeathFloor deathFloor in deathFloors)
            {
                deathFloor.OnEnter += (type) =>
                {
                    if (respawnManager.IsRespawning)
                    {
                        return;
                    }

                    OnEnterDeathFloor(type).Forget();
                };
            }
        }

        private async UniTaskVoid OnEnterDeathFloor(deathType type)
        {
            if (respawnDataOnDeath.LevelResetter == null)
            {
                throw new NoNullAllowedException("チェックポイントが設定されていません！");
            }

            playerController.Kill(type);

            await respawnManager.RespawnPlayer(respawnDataOnDeath, RespawnViewSequence);

            playerController.Revival();
        }

        private async UniTask RespawnViewSequence()
        {
            var behaviour = behaviourNavigator.ActivateBehaviour<LoadingBehaviour>(ViewBehaviourState.Loading);
            await behaviour.SequenceLoading();
            behaviourNavigator.DeactivateBehaviour(ViewBehaviourState.Loading);
        }

        public void Initialize() { }
    }
}