namespace Module.Core.Save
{
    public interface ICloneable<out T>
    {
        T Clone();
    }
}