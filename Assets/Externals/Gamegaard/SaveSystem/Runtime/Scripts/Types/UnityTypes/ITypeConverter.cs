namespace Gamegaard.SavingSystem.Types
{
    public interface ITypeConverter<T>
    {
        T ToOriginalType();
    }
}