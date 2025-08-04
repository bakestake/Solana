using Gamegaard.Timer;
using System;
using UnityEngine;

namespace Gamegaard.FarmSystem
{
    [Serializable]
    public class PlantDryingModule : BasicPlantModule
    {
        [SerializeField] private float dryTime = 10f;

        protected Waiter dryTimer;

        public override void Initialize(PlantModulesHandler modulesHandler, Plant plant)
        {
            base.Initialize(modulesHandler, plant);
            dryTimer = new Waiter(dryTime);
            plant.OnPhaseChanged += OnPhaseChanged;
            Soil.ConditionReceiver.OnConditionAdded += OnSoilConditionChanged;
        }

        public override void Deinitialize()
        {
            plant.OnPhaseChanged -= OnPhaseChanged;
            Soil.ConditionReceiver.OnConditionAdded -= OnSoilConditionChanged;
        }

        public override void UpdateModule()
        {
            if (Soil.HasCondition("Dry") && dryTimer.IsCompleted)
            {
                plant.SetState("Dry");
            }
        }

        private void OnPhaseChanged(PlantPhase phase)
        {
            if (dryTimer.IsCompleted)
            {
                plant.SetState("Dry");
            }
            else
            {
                plant.SetState("Health");
            }
        }

        private void OnSoilConditionChanged(string condition)
        {
            if (condition == "Watered")
            {
                dryTimer.Reset();
                plant.SetState("Health");
            }
        }

        public override void Interact(string command)
        {
            if (command == "Water")
            {
                Soil.RemoveCondition("Dry");
                Soil.AddCondition("Watered");
                dryTimer.Reset();
                plant.SetState("Health");
            }
        }
    }
}