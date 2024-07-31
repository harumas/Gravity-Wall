using Core.Input;
using Module.Core.Input;
using Module.Core.Save;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;

namespace Module.Config
{
    public class ConfigChangedListener
    {
        private readonly InputBinding keyboardBinding = InputBinding.MaskByGroup("Keyboard");
        private readonly InputBinding gamepadBinding = InputBinding.MaskByGroup("GamePad");

        public ConfigChangedListener()
        {
            ConfigData configData = SaveManager<ConfigData>.Instance;
            InputAction mouseAction = InputActionProvider.ActionAsset.FindAction(ActionGuid.Player.Look);

            configData.MouseSensibility.Subscribe(sensibility =>
            {
                mouseAction.ApplyParameterOverride((ScaleVector2Processor param) => param.x, sensibility.x, keyboardBinding);
                mouseAction.ApplyParameterOverride((ScaleVector2Processor param) => param.y, sensibility.y, keyboardBinding);
            });
            
            configData.MouseSensibility.Subscribe(sensibility =>
            {
                mouseAction.ApplyParameterOverride((ScaleVector2Processor param) => param.x, sensibility.x, gamepadBinding);
                mouseAction.ApplyParameterOverride((ScaleVector2Processor param) => param.y, sensibility.y, gamepadBinding);
            });
        }
    }
}