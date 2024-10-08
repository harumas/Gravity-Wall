using Module.Character;
using Module.InputModule;
using R3;
using VContainer;
using VContainer.Unity;

namespace Presentation
{
    /// <summary>
    /// プレイヤー入力を接続するクラス
    /// </summary>
    public class PlayerInputPresenter : IInitializable
    {
        [Inject]
        public PlayerInputPresenter(
            IGameInput gameInput,
            PlayerController playerController,
            CameraController cameraController,
            PlayerTargetSyncer playerTargetSyncer,
            GravitySwitcher gravitySwitcher)
        {
            gameInput.Move.Subscribe(playerController.OnMoveInput).AddTo(playerController);
            gameInput.Jump.Subscribe(_ => playerController.OnJumpInput()).AddTo(playerController);

            gameInput.LookDelta.Subscribe(cameraController.OnRotateCameraInput).AddTo(cameraController);

            gameInput.Move.Subscribe(playerTargetSyncer.OnMoveInput).AddTo(playerTargetSyncer);
            gameInput.Move.Subscribe(gravitySwitcher.OnMoveInput).AddTo(playerTargetSyncer);
        }

        public void Initialize()
        {
        }
    }
}