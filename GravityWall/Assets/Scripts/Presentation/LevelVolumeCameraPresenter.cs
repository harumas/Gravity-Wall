using System;
using CoreModule.Helper;
using Module.Character;
using Module.Gimmick;
using Module.InputModule;
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
            IGameInput gameInput)
        {
            this.cameraController = cameraController;
            this.playerController = playerController;

            Transform playerTransform = playerController.transform;
            var cameras = volumeCameras.GetComponents();

            foreach (var cam in cameras)
            {
                cam.AssignPlayerTransform(playerTransform);
                cam.IsEnabled.Skip(1).Subscribe(OnEnableChanged).AddTo(cam);
                cam.Rotation.Skip(1).Subscribe(cameraController.SetCameraRotation).AddTo(cam);

                cam.IsRotating.Skip(1).Subscribe(isRotating => playerController.IsRotationLocked = isRotating);

                gameInput.CameraRotate.Skip(1).Subscribe(value => cam.EnableAdditionalRotate(value)).AddTo(cam);
            }
        }

        public void Initialize() { }

        private void OnEnableChanged(bool isEnable)
        {
            if (isEnable)
            {
                cameraController.SetFreeCamera(false);
            }
            else
            {
                cameraController.SetFreeCamera(true);
            }
        }
    }
}