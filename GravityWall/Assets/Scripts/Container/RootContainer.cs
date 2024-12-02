using Application;
using Application.Sequence;
using CoreModule.Input;
using CoreModule.Save;
using Module.Config;
using Module.InputModule;
using Presentation;
using UnityEngine;
using UnityEngine.Audio;
using VContainer;
using VContainer.Unity;

namespace Container
{
    /// <summary>
    /// アプリケーション共通のDIコンテナ
    /// </summary>
    public class RootContainer : LifetimeScope
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private InputValueAdjustParameter inputAdjustParameter;
        [SerializeField] private VibrationParameter vibrationParameter;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<ApplicationStarter>();
            
            builder.Register<ConfigLoader>(Lifetime.Singleton);
            builder.Register<SaveManager<ConfigData>>(Lifetime.Singleton);
            builder.Register<ApplicationStopper>(Lifetime.Singleton);
            builder.Register<GameState>(Lifetime.Singleton);
            builder.Register<GamepadVibrator>(Lifetime.Singleton);
            
            builder.RegisterInstance(InputActionProvider.ActionAsset);
            builder.RegisterInstance(inputAdjustParameter);
            builder.RegisterInstance(vibrationParameter);
            builder.RegisterInstance(audioMixer);
        }
    }
}