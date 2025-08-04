using Gamegaard.Timer;
using UnityEngine;

namespace Gamegaard.FarmSystem
{
    [System.Serializable]
    public class SoilWaterModule : BasicSoilModule, IWaterable
    {
        [SerializeField] private GameObject waterSpriteRenderer;

        private readonly Waiter wateredTimer = new Waiter();
        private const string DryConditionName = "Dry";
        private const string WateredConditionName = "Watered";

        public override void Initialize(SoilModulesHandler soilModuleHandler, Soil soil)
        {
            base.Initialize(soilModuleHandler, soil);
            RemoveWater();
        }

        public override void UpdateModule()
        {
            if (wateredTimer.IsCompletedThisFrame() && soil.HasCondition(WateredConditionName))
            {
                RemoveWater();
            }
        }

        private void RemoveWater()
        {
            waterSpriteRenderer.SetActive(false);
            soil.RemoveCondition(WateredConditionName);
            soil.AddCondition(DryConditionName);
        }

        public void Water(float time)
        {
            waterSpriteRenderer.SetActive(true);
            soil.RemoveCondition(DryConditionName);
            soil.AddCondition(WateredConditionName);
            wateredTimer.SetAndReset(time);
        }
    }
}