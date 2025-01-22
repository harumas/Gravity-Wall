using Application.Spawn;
using CoreModule.Helper;
using Module.Gimmick;
using Module.Gimmick.SystemGimmick;
using Module.InputModule;
using Module.Player;
using R3;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Presentation
{
    public class LevelVolumeCameraPresenter : IInitializable
    {
        private readonly CameraController cameraController;
        private readonly PlayerController playerController;

        [Inject]
        public LevelVolumeCameraPresenter(
            ReusableComponents<LevelVolumeCamera> volumeCameras,
            CameraController cameraController,
            PlayerController playerController,
            RespawnManager respawnManager,
            IGameInput gameInput)
        {
            this.cameraController = cameraController;
            this.playerController = playerController;

            Transform playerTransform = playerController.transform;
            var cameras = volumeCameras.GetComponents();

            foreach (var cam in cameras)
            {
                cam.AssignPlayerTransform(playerTransform, cameraController);
                cam.IsEnabled.Skip(1).Subscribe(OnEnableChanged).AddTo(cam);
                cam.Rotation.Skip(1).Subscribe(cameraController.SetCameraRotation).AddTo(cam);

                cam.IsRotating.Skip(1).Subscribe(isRotating => playerController.IsRotationLocked = isRotating);

                gameInput.CameraRotate.Skip(1).Subscribe(value => cam.EnableAdditionalRotate(value)).AddTo(cam);

                respawnManager.IsRespawning.Skip(1).Subscribe(isRespawning =>
                {
                    if (cam.IsEnabled.CurrentValue && !isRespawning)
                    {
                        cameraController.SetFreeCamera(false);
                        cam.SetDirection(cameraController.transform.forward);
                    }
                }).AddTo(cam);
            }
        }

        public void Initialize()
        {
        }

        private void OnEnableChanged(bool isEnable)
        {
            bool isFreeCamera = !isEnable;
            cameraController.SetFreeCamera(isFreeCamera);
        }
    }
}