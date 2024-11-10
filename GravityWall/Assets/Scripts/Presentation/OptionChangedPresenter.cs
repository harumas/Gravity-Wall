using CoreModule.Save;
using Module.Config;
using R3;
using VContainer.Unity;
using View;

namespace Presentation
{
    public class OptionChangedPresenter : IInitializable
    {
        private readonly ConfigData configData;
        private readonly OptionBehaviour optionBehaviour;

        public OptionChangedPresenter(SaveManager<ConfigData> configManager, ViewBehaviourNavigator behaviourNavigator)
        {
            configData = configManager.Data;
            optionBehaviour = behaviourNavigator.GetBehaviour<OptionBehaviour>(ViewBehaviourType.Option);
        }

        public void Initialize()
        {
            OptionView optionView = optionBehaviour.OptionView;
            
            optionBehaviour.IsActive.Subscribe(isActive =>
            {
                if (isActive)
                {
                    optionView.SetBgmVolume(configData.BgmVolume.Value);        
                    optionView.SetSeVolume(configData.SeVolume.Value);
                    optionView.SetControllerSensibility(configData.PadSensibility.Value);
                    optionView.SetVibrationToggle(configData.Vibration.Value);
                }
            }).AddTo(optionBehaviour);

            optionView.OnBgmVolumeChanged.Subscribe(value => configData.BgmVolume.Value = value).AddTo(optionView);
            optionView.OnSeVolumeChanged.Subscribe(value => configData.SeVolume.Value = value).AddTo(optionView);
            optionView.OnControllerSensibilityChanged.Subscribe(value => configData.PadSensibility.Value = value).AddTo(optionView);
            optionView.OnVibrationToggleChanged.Subscribe(value => configData.Vibration.Value = value).AddTo(optionView);
        }
    }
}