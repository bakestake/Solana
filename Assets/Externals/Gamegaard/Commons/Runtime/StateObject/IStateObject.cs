namespace Gamegaard
{
    public interface IStateObject
    {
        void Activate();
        void Deactivate();
        void Toggle();
    }
}