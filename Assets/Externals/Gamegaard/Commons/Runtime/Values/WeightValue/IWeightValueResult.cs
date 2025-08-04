namespace Gamegaard.CustomValues
{
    public interface IWeightValueResult<T> : IWeightValue
    {
        T GetValue();
    }
}