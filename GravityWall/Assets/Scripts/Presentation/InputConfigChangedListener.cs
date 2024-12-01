using CoreModule.Save;
using CoreModule.Input;
using Module.Config;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;
using VContainer;
using VContainer.Unity;

namespace Presentation
{
    /// <summary>
    /// 入力設定の変更を反映するクラス
    /// </summary>
    public class InputConfigChangedListener : IStartable
    {
        private readonly InputBinding keyboardBinding = InputBinding.MaskByGroup("Keyboard");
        private readonly InputBinding gamepadBinding = InputBinding.MaskByGroup("Gamepad");

        private readonly ConfigData configData;
        private readonly InputAction lookAction;

        [Inject]
        public InputConfigChangedListener(SaveManager<ConfigData> saveManager, InputActionAsset inputActionAsset)
        {
            configData = saveManager.Data;

            //視点移動のInputActionを取得する
            lookAction = inputActionAsset.FindAction(ActionGuid.Player.Look);
        }

        public void Start()
        {
            //初期値を反映する
            ApplyParameters(configData);

            //更新イベントを登録する
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
            lookAction.ApplyParameterOverride((ScaleVector2Processor param) => param.x, sensibility.x, keyboardBinding);
            lookAction.ApplyParameterOverride((ScaleVector2Processor param) => param.y, sensibility.y, keyboardBinding);
        }

        private void UpdateGamePadSensibility(Vector2 sensibility)
        {
            lookAction.ApplyParameterOverride((ScaleVector2Processor param) => param.x, sensibility.x, gamepadBinding);
            lookAction.ApplyParameterOverride((ScaleVector2Processor param) => param.y, sensibility.y, gamepadBinding);
        }
    }
}