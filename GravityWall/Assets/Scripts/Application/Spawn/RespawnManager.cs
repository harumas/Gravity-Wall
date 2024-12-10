using System;
using Cysharp.Threading.Tasks;
using Module.Gimmick;
using Module.Gravity;
using Module.Player;
using UnityEngine;
using VContainer;

namespace Application.Spawn
{
    /// <summary>
    /// リスポーン機能を提供するクラス
    /// </summary>
    public class RespawnManager
    {
        private readonly PlayerController playerController;
        private readonly CameraController cameraController;
        private readonly PlayerTargetSyncer playerTargetSyncer;
        private readonly GravitySwitcher gravitySwitcher;
        private bool isRespawning;

        /// <summary>
        /// リスポーン中かどうか
        /// </summary>
        public bool IsRespawning => isRespawning;

        [Inject]
        public RespawnManager(
            PlayerController playerController,
            CameraController cameraController,
            PlayerTargetSyncer playerTargetSyncer,
            GravitySwitcher gravitySwitcher)
        {
            this.playerController = playerController;
            this.cameraController = cameraController;
            this.playerTargetSyncer = playerTargetSyncer;
            this.gravitySwitcher = gravitySwitcher;
        }

        public async UniTask RespawnPlayer(RespawnContext respawnContext, Func<UniTask> respawningTask)
        {
            isRespawning = true;
            LockPlayer();

            //リスポーン演出があれば実行
            var task = respawningTask != null ? respawningTask() : UniTask.CompletedTask;
            await task;

            //重力の復元
            WorldGravity.Instance.SetValue(respawnContext.Gravity);

            //レベル上のオブジェクトの復元
            respawnContext.LevelResetter?.ResetLevel();

            UnlockPlayer(respawnContext);
            isRespawning = false;
        }

        public void LockPlayer()
        {
            playerTargetSyncer.Lock();
            playerController.Lock();
            playerTargetSyncer.Reset();
            gravitySwitcher.Disable();
        }

        public void UnlockPlayer(RespawnContext respawnContext)
        {
            playerController.Revival();
            playerTargetSyncer.Unlock();
            playerController.Unlock();
            playerController.transform.SetPositionAndRotation(respawnContext.Position, respawnContext.Rotation);
            cameraController.SetCameraRotation(respawnContext.Rotation);
            playerTargetSyncer.SetRotation(respawnContext.Rotation);
            gravitySwitcher.Enable();

            if (respawnContext.Velocity != Vector3.zero)
            {
                float jumpingGravity = playerController.Parameter.JumpingGravity;
                playerController.AddForce(respawnContext.Velocity, ForceMode.VelocityChange, jumpingGravity, false);
            }
        }
    }
}