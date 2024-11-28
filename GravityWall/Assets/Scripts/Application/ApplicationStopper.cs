namespace Application
{
    /// <summary>
    /// アプリケーションの停止を行うクラス
    /// </summary>
    public class ApplicationStopper
    {
        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
    }
}