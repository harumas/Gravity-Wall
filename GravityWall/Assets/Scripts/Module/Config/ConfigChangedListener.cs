using Core.Input;
using Module.Core.Input;
using Module.Core.Save;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;
using VContainer;
using VContainer.Unity;

namespace Module.Config
{
    public class ConfigChangedListener : IInitializable
    {
        private readonly InputBinding keyboardBinding = InputBinding.MaskByGroup("Keyboard");
        private readonly InputBinding gamepadBinding = InputBinding.MaskByGroup("GamePad");

        private readonly ConfigData configData;
        private readonly InputAction mouseAction;

        [Inject]
        public ConfigChangedListener()
        {
            configData = SaveManager<ConfigData>.Instance;
            mouseAction = InputActionProvider.ActionAsset.FindAction(ActionGuid.Player.Look);
        }

        public void Initialize()
        {
            ApplyParameters(configData);

            configData.MouseSensibility.Subscribe(UpdateMouseSensibility);
            configData.PadSensibility.Subscribe(UpdateGamePadSensibility);
        }

        private void ApplyParameters(ConfigData configData)
        {
            UpdateMouseSensibility(configData.MouseSensibility.CurrentValue);
            UpdateGamePadSensibility(configData.PadSensibility.CurrentValue);
        }

        private void UpdateMouseSensibility(Vector2 sensibility)
        {
            mouseAction.ApplyParameterOverride((ScaleVector2Processor param) => param.x, sensibility.x, keyboardBinding);
            mouseAction.ApplyParameterOverride((ScaleVector2Processor param) => param.y, sensibility.y, keyboardBinding);
        }

        private void UpdateGamePadSensibility(Vector2 sensibility)
        {
            mouseAction.ApplyParameterOverride((ScaleVector2Processor param) => param.x, sensibility.x, gamepadBinding);
            mouseAction.ApplyParameterOverride((ScaleVector2Processor param) => param.y, sensibility.y, gamepadBinding);
        }
    }
}