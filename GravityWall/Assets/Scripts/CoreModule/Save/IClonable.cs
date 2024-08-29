namespace CoreModule.Save
{
    /// <summary>
    /// 複製可能なクラスを表すインターフェース
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICloneable<out T>
    {
        T Clone();
    }
}