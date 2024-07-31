using Module.Config;
using VContainer;
using VContainer.Unity;

namespace GameMain
{
    public class InGameContainer : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<ConfigChangedListener>();
        }
    }
}