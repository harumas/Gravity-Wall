using VContainer;
using VContainer.Unity;

namespace GameMain
{
    public class RootContainer : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameStarter>();
        }
    }
}