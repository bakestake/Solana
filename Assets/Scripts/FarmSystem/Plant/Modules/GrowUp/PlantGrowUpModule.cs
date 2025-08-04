using Gamegaard.Timer;
using System;

namespace Gamegaard.FarmSystem
{
    [Serializable]
    public class PlantGrowUpModule : BasicPlantModule
    {
        private readonly BasicTimer growUpTimer = new BasicTimer();
        protected SoilModulesHandler soilModulesHandler;

        public override void Initialize(PlantModulesHandler modulesHandler, Plant plant)
        {
            base.Initialize(modulesHandler, plant);
            plant.OnPhaseChanged += OnPhaseChanged;
            Soil.ConditionReceiver.OnConditionAdded += OnSoilConditionChanged;
            Soil.ConditionReceiver.OnConditionRemoved += OnSoilConditionChanged;
            soilModulesHandler = plant.Soil.GetComponent<SoilModulesHandler>();
            OnPhaseChanged(plant.Phase);
            CheckFertilizer();
        }

        public override void Deinitialize()
        {
            plant.OnPhaseChanged -= OnPhaseChanged;
            Soil.ConditionReceiver.OnConditionAdded -= OnSoilConditionChanged;
            Soil.ConditionReceiver.OnConditionRemoved -= OnSoilConditionChanged;
        }

        public override void UpdateModule()
        {
            if (Soil.HasCondition("Weeds") || Soil.HasCondition("Dry")) return;

            if (growUpTimer.CheckAndUpdateTimer() && plant.PhaseIndex < plant.Data.Phases.Length - 1)
            {
                GrowUp();
            }
        }

        private void GrowUp()
        {
            plant.SetToPhase(plant.PhaseIndex + 1);
        }

        private void OnPhaseChanged(PlantPhase phase)
        {
            growUpTimer.SetTimer(phase.TimeToUpgrade);
        }

        private void OnSoilConditionChanged(string condition)
        {
            if (soilModulesHandler == null) return;

            if (condition == "Fertilized")
            {
                CheckFertilizer();
            }
        }

        private void CheckFertilizer()
        {
            float growSpeed = 1f;
            if (Soil.HasCondition("Fertilized"))
            {
                foreach (ISoilModule module in soilModulesHandler.Modules)
                {
                    if (module is IFertilizable fertilizable)
                    {
                        growSpeed = fertilizable.CurrentFertilizer.growUpMultiplier;
                        break;
                    }
                }
            }
            growUpTimer.SetTimeScale(growSpeed);
        }

        public override void Interact(string command)
        {
            if (command == "Fertilize")
            {
                Soil.AddCondition("Fertilized");
            }
        }

        private void SimulateGrowth(float secondsPassed)
        {
            int maxPhases = plant.Data.Phases.Length;
            int currentPhase = plant.PhaseIndex;

            while (currentPhase < maxPhases - 1)
            {
                float timeToUpgrade = plant.Data.Phases[currentPhase].TimeToUpgrade;
                float estimatedCycles = secondsPassed / timeToUpgrade;

                if (estimatedCycles >= 1f)
                {
                    secondsPassed -= timeToUpgrade;
                    currentPhase++;
                    plant.SetToPhase(currentPhase);
                }
                else
                {
                    growUpTimer.SetTimer(timeToUpgrade);
                    growUpTimer.UpdateTimer(secondsPassed);
                    break;
                }
            }
        }

        public override object CaptureState()
        {
            return new PlantGrowUpSaveData
            {
                phaseIndex = plant.PhaseIndex,
                elapsedTime = growUpTimer.ElapsedTime,
                lastSaveTime = DateTime.UtcNow.ToString("o")
            };
        }

        public override void RestoreState(object state)
        {
            if (state is not PlantGrowUpSaveData data) return;

            plant.SetToPhase(data.phaseIndex);
            float secondsPassed = (float)(DateTime.UtcNow - DateTime.Parse(data.lastSaveTime)).TotalSeconds;
            float totalElapsed = data.elapsedTime + secondsPassed;
            SimulateGrowth(totalElapsed);
        }
    }
}