using Module.Character;
using Module.Gimmick;
using Module.InputModule;
using R3;
using UnityEngine;
using VContainer.Unity;

namespace Presentation
{
    public class LevelVolumeCameraPresenter : IInitializable
    {
        private readonly CameraController cameraController;
        private readonly PlayerController playerController;

        public LevelVolumeCameraPresenter(CameraController cameraController, PlayerController playerController, IGameInput gameInput)
        {
            this.cameraController = cameraController;
            this.playerController = playerController;

            Transform playerTransform = playerController.transform;
            LevelVolumeCamera[] cameras = Object.FindObjectsByType<LevelVolumeCamera>(FindObjectsSortMode.None);

            foreach (var cam in cameras)
            {
                cam.AssignPlayerTransform(playerTransform);
                cam.IsEnabled.Subscribe(OnEnableChanged).AddTo(cam);
                cam.Rotation.Subscribe(cameraController.SetCameraRotation).AddTo(cam);

                cam.IsRotating.Subscribe(isRotating => playerController.IsRotationLocked = isRotating);

                gameInput.CameraRotate.Subscribe(value => cam.EnableAdditionalRotate(value)).AddTo(cam);
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