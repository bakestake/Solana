namespace Gamegaard.RuntimeDebug
{
    public interface ITypeParser<T>
    {
        T Parse(string value);
    }
}