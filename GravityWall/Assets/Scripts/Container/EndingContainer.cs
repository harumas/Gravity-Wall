using Presentation;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using View;

namespace Container
{
    public class EndingContainer : LifetimeScope
    {
        [SerializeField] private EndingView endingView;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<EndingPresenter>();
            builder.RegisterInstance(endingView);
        }
    }
}