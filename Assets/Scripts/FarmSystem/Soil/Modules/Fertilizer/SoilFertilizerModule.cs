using Gamegaard.Timer;
using UnityEngine;

namespace Gamegaard.FarmSystem
{
    [System.Serializable]
    public class SoilFertilizerModule : BasicSoilModule, IFertilizable
    {
        [SerializeField] private GameObject fertilizerSpriteRenderer;

        private const string ConditionName = "Fertilized";
        private readonly Waiter fertilizedTimer = new Waiter();

        public FertilizerData CurrentFertilizer { get; private set; }

        public override void UpdateModule()
        {
            if (fertilizedTimer.IsCompletedThisFrame())
            {
                RemoveFertilizer();
            }
        }

        public void RemoveFertilizer()
        {
            fertilizerSpriteRenderer.SetActive(false);
            soil.RemoveCondition(ConditionName);
        }

        public void SetFertilized(FertilizerData fertilizerData)
        {
            CurrentFertilizer = fertilizerData;
            fertilizedTimer.SetAndReset(fertilizerData.durationTime);
            fertilizerSpriteRenderer.SetActive(true);
            soil.AddCondition(ConditionName);
        }
    }
}