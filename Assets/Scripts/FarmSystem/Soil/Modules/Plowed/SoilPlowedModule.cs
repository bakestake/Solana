using Gamegaard.Timer;
using UnityEngine;

namespace Gamegaard.FarmSystem
{
    [System.Serializable]
    public class SoilPlowedModule : BasicSoilModule
    {
        [SerializeField] private float durationInSeconds = 20;

        private readonly BasicTimer plowedTimer = new BasicTimer();

        public override void Initialize(SoilModulesHandler soilModuleHandler, Soil soil)
        {
            base.Initialize(soilModuleHandler, soil);
            plowedTimer.SetTimer(durationInSeconds);
        }

        public override void Deinitialize() { }

        public override void UpdateModule()
        {
            if (soil.HasCondition("HasPlant") || soil.HasCondition("Watered") || soil.HasCondition("Fertilized"))
            {
                plowedTimer.Reset();
            }

            if (plowedTimer.CheckAndUpdateTimer())
            {
                RemovePlowed();
            }
        }

        public void RemovePlowed()
        {
            soil.DestroySoil();
        }

        public void SetPlowed()
        {
            plowedTimer.SetTimer(durationInSeconds);
        }
    }
}