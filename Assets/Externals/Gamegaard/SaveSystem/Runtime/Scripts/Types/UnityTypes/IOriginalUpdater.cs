namespace Gamegaard.SavingSystem.Types
{
    public interface IOriginalUpdater<T> where T : class
    {
        void UpdateOriginal(T original);
    }
}