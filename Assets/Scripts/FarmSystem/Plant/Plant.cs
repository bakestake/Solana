using System;
using UnityEngine;
using UnityEngine.Events;

namespace Gamegaard.FarmSystem
{
    public class Plant : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer render;
        [SerializeField] private UnityEvent<PlantData, object> OnHarvested;

        public bool IsInitialized { get; private set; }
        public int PhaseIndex { get; private set; }
        public Soil Soil { get; private set; }
        public PlantPhase Phase { get; private set; }
        public PlantData Data { get; private set; }
        private StateConditionReceiver ConditionReceiver => Soil.ConditionReceiver;

        public event Action<PlantPhase> OnPhaseChanged;

        public void Initialize(Soil soil, PlantData plantData)
        {
            name = $"Plant [{plantData.PlantName}]";
            soil.AddCondition("HasPlant");
            IsInitialized = true;
            Data = plantData;
            Soil = soil;
            SetToPhase(0);
        }

        public void SetToPhase(int phaseIndex)
        {
            PhaseIndex = phaseIndex;
            Phase = Data.Phases[phaseIndex];
            OnPhaseChanged?.Invoke(Phase);
            SetState("Health");
        }

        public void SetState(string stateName)
        {
            if (Phase.GetState(stateName, out PlantState state))
            {
                render.sprite = state.sprite;
            }
        }

        public bool TryHarvest(object caster)
        {
            if (Phase.CanHarvest)
            {
                HarvestLoot(caster);
                return true;
            }
            return false;
        }

        private void HarvestLoot(object caster)
        {
            OnHarvested?.Invoke(Data, caster);

            if (Phase.PhaseAfterHarvest == -1)
            {
                foreach (PlantReward reward in Phase.Loot)
                {
                    reward.ReceiveReward(this, caster);
                }
                Soil.RemoveCondition("HasPlant");
                Destroy(gameObject);
            }
            else
            {
                SetToPhase(Phase.PhaseAfterHarvest);
            }
        }

        public bool HasCondition(string condition) => ConditionReceiver.ContainsCondition(condition);
        public void SetCondition(string condition) => ConditionReceiver.AddCondition(condition);
        public void RemoveCondition(string condition) => ConditionReceiver.RemoveCondition(condition);
    }
}