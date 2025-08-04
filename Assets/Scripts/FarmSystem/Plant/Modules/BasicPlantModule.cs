namespace Gamegaard.FarmSystem
{
    public abstract class BasicPlantModule : IPlantModule
    {
        protected Plant plant;
        protected PlantModulesHandler modulesHandler;
        protected Soil Soil => plant.Soil;

        public virtual void Initialize(PlantModulesHandler modulesHandler, Plant plant)
        {
            this.modulesHandler = modulesHandler;
            this.plant = plant;
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