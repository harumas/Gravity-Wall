using System;
using System.Data;
using CoreModule.Helper;
using Cysharp.Threading.Tasks;
using Module.Character;
using Module.Gimmick;
using Module.Gravity;
using R3;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace Application.Sequence
{
    public class RespawnManager
    {
        private readonly PlayerController playerController;
        private readonly CameraController cameraController;
        private readonly PlayerTargetSyncer playerTargetSyncer;
        private readonly GravitySwitcher gravitySwitcher;
        private RespawnContext respawnData;
        private bool isRespawning;

        /// <summary>
        /// Viewのリスポーン演出を待機するイベント
        /// </summary>
        public event Func<UniTask> RespawnViewSequence;

        [Inject]
        public RespawnManager(
            ReusableComponents<SavePoint> savePoints,
            ReusableComponents<DeathFloor> deathFloors,
            PlayerController playerController,
            CameraController cameraController,
            PlayerTargetSyncer playerTargetSyncer,
            GravitySwitcher gravitySwitcher)
        {
            this.playerController = playerController;
            this.cameraController = cameraController;
            this.playerTargetSyncer = playerTargetSyncer;
            this.gravitySwitcher = gravitySwitcher;

            Initialize(savePoints, deathFloors);
        }

        private void Initialize(ReusableComponents<SavePoint> savePointComponents, ReusableComponents<DeathFloor> deathFloorComponents)
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
                    if (isRespawning)
                    {
                        return;
                    }

                    OnEnterDeathFloor().Forget();
                };
            }
        }

        private void OnSave(RespawnContext respawnContext)
        {
            respawnData = respawnContext;
        }

        private async UniTaskVoid OnEnterDeathFloor()
        {
            if (respawnData.LevelResetter == null)
            {
                throw new NoNullAllowedException("チェックポイントが設定されていません！");
            }

            isRespawning = true;

            //プレイヤーの無効化
            DisablePlayer();

            //リスポーン演出があれば実行
            if (RespawnViewSequence != null)
            {
                await RespawnViewSequence();
            }

            //重力の復元
            WorldGravity.Instance.SetValue(respawnData.Gravity);

            //レベル上のオブジェクトの復元
            respawnData.LevelResetter.ResetLevel();

            //プレイヤーの有効化
            EnablePlayer();

            isRespawning = false;
        }

        private void DisablePlayer()
        {
            playerController.Kill();
            playerTargetSyncer.Reset();
            gravitySwitcher.Disable();
        }

        private void EnablePlayer()
        {
            playerController.Respawn();
            playerController.transform.SetPositionAndRotation(respawnData.Position, respawnData.Rotation);
            cameraController.SetCameraRotation(respawnData.Rotation);
            playerTargetSyncer.SetRotation(respawnData.Rotation);
            gravitySwitcher.Enable();
        }
    }
}