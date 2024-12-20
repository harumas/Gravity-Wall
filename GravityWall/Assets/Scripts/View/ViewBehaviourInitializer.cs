using VContainer;
using VContainer.Unity;
using View;

namespace Application
{
    /// <summary>
    /// ViewBehaviourNavigatorを初期状態に遷移するクラス
    /// </summary>
    public class ViewBehaviourInitializer : IPostStartable
    {
        private readonly ViewBehaviourNavigator navigator;

        [Inject]
        public ViewBehaviourInitializer(ViewBehaviourNavigator navigator)
        {
            this.navigator = navigator;
        }

        public void PostStart()
        {
            navigator.ActivateBehaviour(ViewBehaviourState.Title);
        }
    }
}