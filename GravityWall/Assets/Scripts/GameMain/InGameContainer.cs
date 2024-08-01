using Module.Config;
using Module.Core.Input;
using Module.Core.Save;
using VContainer;
using VContainer.Unity;

namespace GameMain
{
    public class InGameContainer : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            if (!GameBoot.IsBooted)
            {
                return;
            }
            
            builder.RegisterEntryPoint<ConfigChangedListener>();
            
            builder.RegisterInstance(SaveManager<ConfigData>.Instance);
        }
    }
}