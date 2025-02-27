﻿using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Module.Gimmick.SystemGimmick;
using Module.Gravity;
using Module.Player;
using R3;
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
        private readonly ReactiveProperty<bool> isRespawning;

        /// <summary>
        /// リスポーン中かどうか
        /// </summary>
        public ReadOnlyReactiveProperty<bool> IsRespawning => isRespawning;

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

            isRespawning = new ReactiveProperty<bool>();
        }

        public async UniTask RespawnPlayer(RespawnContext respawnContext, Func<CancellationToken, UniTask> respawningTask)
        {
            isRespawning.Value = true;
            LockPlayer();

            //リスポーン演出があれば実行
            var task = respawningTask != null ? respawningTask(playerController.destroyCancellationToken) : UniTask.CompletedTask;
            await task;

            //重力の復元
            WorldGravity.Instance.SetValue(respawnContext.Gravity);

            //レベル上のオブジェクトの復元
            respawnContext.LevelResetter?.ResetLevel();

            UnlockPlayer(respawnContext);
            isRespawning.Value = false;
        }

        public void LockPlayer()
        {
            playerTargetSyncer.Lock();
            playerController.Lock(RigidbodyConstraints.FreezeRotation, true);
            playerTargetSyncer.Reset();
            gravitySwitcher.Disable();
            cameraController.SetFreeCamera(false);
        }

        public void UnlockPlayer(RespawnContext respawnContext)
        {
            playerController.Revival();
            playerTargetSyncer.Unlock();
            playerController.Unlock();
            playerController.transform.SetPositionAndRotation(respawnContext.Position, respawnContext.Rotation);
            cameraController.SetCameraRotation(respawnContext.Rotation);
            cameraController.SetFreeCamera(true);
            playerTargetSyncer.SetRotation(respawnContext.Rotation);

            if (respawnContext.IsGravitySwitcherEnabled)
            {
                gravitySwitcher.Enable();
            }

            if (respawnContext.Velocity != Vector3.zero)
            {
                float jumpingGravity = playerController.Parameter.JumpingGravity;
                playerController.DoJump(respawnContext.Velocity, jumpingGravity);
            }
        }
    }
}