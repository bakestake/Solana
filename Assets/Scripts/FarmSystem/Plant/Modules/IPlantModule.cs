namespace Gamegaard.FarmSystem
{
    public interface IPlantModule : ICommandInteraction
    {
        void Initialize(PlantModulesHandler modulesHandler, Plant plant);
        void Deinitialize();
        void UpdateModule();

        object CaptureState();
        void RestoreState(object state);
    }
}