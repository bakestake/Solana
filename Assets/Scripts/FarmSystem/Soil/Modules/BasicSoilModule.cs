namespace Gamegaard.FarmSystem
{
    public class BasicSoilModule : ISoilModule
    {
        protected SoilModulesHandler soilModuleHandler;
        protected Soil soil;

        public virtual void Initialize(SoilModulesHandler soilModuleHandler, Soil soil)
        {
            this.soilModuleHandler = soilModuleHandler;
            this.soil = soil;
        }

        public virtual void Deinitialize()
        {
        }

        public virtual void UpdateModule()
        {
        }

        public virtual void Interact(string command)
        {
        }

        public virtual object CaptureState()
        {
            return null;
        }

        public virtual void RestoreState(object state)
        {
        }
    }
}