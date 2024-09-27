using Application;
using CoreModule.Input;
using CoreModule.Save;
using Module.Config;
using VContainer;
using VContainer.Unity;

namespace Container
{
    /// <summary>
    /// アプリケーション共通のDIコンテナ
    /// </summary>
    public class RootContainer : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameStarter>();
            
            builder.Register<ConfigLoader>(Lifetime.Singleton);
            builder.Register<SaveManager<ConfigData>>(Lifetime.Singleton);
            builder.RegisterInstance(InputActionProvider.ActionAsset);
        }
    }
}