using VContainer;
using VContainer.Unity;

namespace Container
{
    /// <summary>
    /// ゲーム外からコンテナにアクセス出来るようにするクラス
    /// デバッグ以外で使用しない
    /// </summary>
    public class ExternalAccessor : IInitializable
    {
        public static IObjectResolver Resolver { get; private set; }

        [Inject]
        public ExternalAccessor(IObjectResolver objectResolver)
        {
            Resolver = objectResolver;
        }

        public void Initialize()
        {
        }
    }
}