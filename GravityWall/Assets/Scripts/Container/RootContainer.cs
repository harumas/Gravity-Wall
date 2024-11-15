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
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameStarter>();
            
            builder.Register<ConfigLoader>(Lifetime.Singleton);
            builder.Register<SaveManager<ConfigData>>(Lifetime.Singleton);
            builder.Register<GameStopper>(Lifetime.Singleton);
            builder.Register<GameState>(Lifetime.Singleton);
            builder.RegisterInstance(InputActionProvider.ActionAsset);
            builder.RegisterInstance(inputAdjustParameter);
            builder.RegisterInstance(audioMixer);
        }
    }
}