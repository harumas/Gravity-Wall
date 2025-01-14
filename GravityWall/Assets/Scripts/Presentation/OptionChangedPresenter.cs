using CoreModule.Save;
using Module.Config;
using R3;
using VContainer;
using VContainer.Unity;
using View;

namespace Presentation
{
    public class OptionChangedPresenter : IStartable
    {
        private readonly ConfigData configData;
        private readonly OptionBehaviour optionBehaviour;

        [Inject]
        public OptionChangedPresenter(SaveManager<ConfigData> configManager, ViewBehaviourNavigator behaviourNavigator)
        {
            configData = configManager.Data;
            optionBehaviour = behaviourNavigator.GetBehaviour<OptionBehaviour>(ViewBehaviourState.Option);
        }

        public void Start()
        {
            OptionView optionView = optionBehaviour.OptionView;

            optionBehaviour.OnActiveStateChanged.Subscribe(context =>
            {
                if (context.isActive)
                {
                    optionView.SetBgmVolume(configData.BgmVolume.Value);
                    optionView.SetSeVolume(configData.SeVolume.Value);
                    optionView.SetControllerSensibility(configData.PadSensibility.Value);
                    optionView.SetVibrationToggle(configData.Vibration.Value);
                }
            }).AddTo(optionBehaviour);

            optionView.OnBgmVolumeChanged.Skip(1).Subscribe(value => configData.BgmVolume.Value = value).AddTo(optionView);
            optionView.OnSeVolumeChanged.Skip(1).Subscribe(value => configData.SeVolume.Value = value).AddTo(optionView);
            optionView.OnControllerSensibilityChanged.Skip(2).Subscribe(value => { configData.PadSensibility.Value = value; }).AddTo(optionView);
            optionView.OnVibrationToggleChanged.Skip(1).Subscribe(value => configData.Vibration.Value = value).AddTo(optionView);
        }
    }
}