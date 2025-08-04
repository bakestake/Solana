namespace Gamegaard.FarmSystem
{
    public interface ISoilModule
    {
        void Initialize(SoilModulesHandler soilModuleHandler, Soil soil);
        void Deinitialize();
        void UpdateModule();
        object CaptureState();
        void RestoreState(object state);
    }
}