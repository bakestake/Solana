using Gamegaard.Timer;
using UnityEngine;

namespace Gamegaard.FarmSystem
{
    [System.Serializable]
    public class SoilWeedsModule : BasicSoilModule, IWeedsable
    {
        [SerializeField] private SpriteRenderer weedsSpriteRenderer;
        private readonly BasicTimer weedsTimer = new BasicTimer();
        private const string ConditionName = "Weeds";

        public override void Initialize(SoilModulesHandler soilModuleHandler, Soil soil)
        {
            base.Initialize(soilModuleHandler, soil);
            ResetWeedsTimer();
        }

        public override void UpdateModule()
        {
            if (soil.HasCondition("HasPlant") && weedsTimer.CheckAndUpdateTimer())
            {
                AddWeeds();
            }
        }

        public void RemoveWeeds()
        {
            soil.RemoveCondition(ConditionName);
            weedsSpriteRenderer.enabled = false;
            ResetWeedsTimer();
        }

        private void AddWeeds()
        {
            soil.AddCondition(ConditionName);
            weedsSpriteRenderer.enabled = true;
        }

        private void ResetWeedsTimer()
        {
            weedsTimer.SetTimer(Random.Range(15, 20));
        }
    }
}