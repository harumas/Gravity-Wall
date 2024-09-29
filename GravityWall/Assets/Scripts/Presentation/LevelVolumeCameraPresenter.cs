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
        public LevelVolumeCameraPresenter(CameraController cameraController, PlayerController playerController, IGameInput gameInput)
        {
            Transform playerTransform = playerController.transform;
            LevelVolumeCamera[] cameras = Object.FindObjectsByType<LevelVolumeCamera>(FindObjectsSortMode.None);

            foreach (var cam in cameras)
            {
                cam.AssignPlayerTransform(playerTransform, cameraController, playerController);

                gameInput.CameraRotate.Subscribe(value => cam.EnableAdditionalRotate(value)).AddTo(cam);
            }
        }

        public void Initialize()
        {
        }
    }
}