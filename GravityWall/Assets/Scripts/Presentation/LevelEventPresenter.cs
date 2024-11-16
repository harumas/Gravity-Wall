using System;
using System.Data;
using System.Threading;
using Application.Sequence;
using CoreModule.Helper;
using Cysharp.Threading.Tasks;
using Module.Character;
using Module.Gimmick;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using View;

namespace Presentation
{
    public class LevelEventPresenter : IInitializable, IDisposable
    {
        private readonly RespawnManager respawnManager;
        private readonly PlayerController playerController;
        private readonly ViewBehaviourNavigator behaviourNavigator;
        private readonly CancellationTokenSource eventCanceller;
        private RespawnContext respawnData;

        [Inject]
        public LevelEventPresenter(RespawnManager respawnManager,
            ViewBehaviourNavigator behaviourNavigator,
            PlayerController playerController,
            ReusableComponents<SavePoint> savePointComponents,
            ReusableComponents<DeathFloor> deathFloorComponents,
            ReusableComponents<LevelVolumeCamera> volumeCameras,
            PauseBehaviour pauseBehaviour)
        {
            this.respawnManager = respawnManager;
            this.behaviourNavigator = behaviourNavigator;
            this.playerController = playerController;
            eventCanceller = new CancellationTokenSource();
            
            pauseBehaviour.PauseView.OnRestartButtonPressed.Subscribe(_ =>
            {
                
                behaviourNavigator.DeactivateBehaviour(ViewBehaviourState.Pause);
                respawnManager.RespawnPlayer(respawnData, null).Forget();

                foreach (LevelVolumeCamera camera in volumeCameras.GetComponents())
                {
                    camera.SyncDirection();
                }
            }).AddTo(eventCanceller.Token);

            SubscribeComponents(savePointComponents, deathFloorComponents);
        }

        private void OnSave(RespawnContext respawnContext)
        {
            respawnData = respawnContext;
        }

        private void SubscribeComponents(
            ReusableComponents<SavePoint> savePointComponents,
            ReusableComponents<DeathFloor> deathFloorComponents
        )
        {
            var savePoints = savePointComponents.GetComponents();
            var deathFloors = deathFloorComponents.GetComponents();

            //セーブポイントのイベント登録
            foreach (SavePoint savePoint in savePoints)
            {
                savePoint.OnEnterPoint += OnSave;
            }

            //死亡床のイベント登録
            foreach (DeathFloor deathFloor in deathFloors)
            {
                deathFloor.OnEnter += () =>
                {
                    if (respawnManager.IsRespawning)
                    {
                        return;
                    }

                    OnEnterDeathFloor().Forget();
                };
            }
        }

        private async UniTaskVoid OnEnterDeathFloor()
        {
            if (respawnData.LevelResetter == null)
            {
                throw new NoNullAllowedException("チェックポイントが設定されていません！");
            }

            playerController.Kill();

            await respawnManager.RespawnPlayer(respawnData, RespawnViewSequence);

            playerController.Revival();
        }

        private async UniTask RespawnViewSequence()
        {
            var behaviour = behaviourNavigator.ActivateBehaviour<LoadingBehaviour>(ViewBehaviourState.Loading);
            await behaviour.SequenceLoading();
            behaviourNavigator.DeactivateBehaviour(ViewBehaviourState.Loading);
        }

        public void Initialize()
        {
        }

        public void Dispose()
        {
            eventCanceller?.Dispose();
        }
    }
}