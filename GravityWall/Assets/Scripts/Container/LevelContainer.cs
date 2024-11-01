using Application.Sequence;
using Presentation;
using VContainer;
using VContainer.Unity;

namespace Container
{
    public class LevelContainer : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<LevelVolumeCameraPresenter>();
            
            builder.Register<RespawnManager>(Lifetime.Singleton);
        }
    }
}