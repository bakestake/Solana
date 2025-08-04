using Gamegaard.CustomValues;
using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard.FarmSystem
{
    [System.Serializable]
    public class PlantPhase
    {
        [SerializeField] private string phaseName;
        [Min(0)]
        [SerializeField] private float timeToUpgrade;
        [SerializeField] private PlantState[] states = new PlantState[] 
        {
            new PlantState("Health")
        };

        [Header("Harvest")]
        [SerializeField] private bool canHarvest;
        [Min(-1)]
        [SerializeField] private int phaseAfterHarvest = -1;
        [SerializeField] private SerializableList<PlantReward> loot;

        public string PhaseName => phaseName;
        public float TimeToUpgrade => timeToUpgrade;
        public bool CanHarvest => canHarvest;
        public IReadOnlyList<PlantReward> Loot => loot;
        public int PhaseAfterHarvest => phaseAfterHarvest;
        public PlantState[] States => states;

        //TODO: Maybe use dictionary or better performance check?
        public bool GetState(string stateName, out PlantState state)
        {
            foreach (PlantState plantState in states)
            {
                if (plantState.stateName == stateName)
                {
                    state = plantState;
                    return true;
                }
            }
            state = default;
            return false;
        }
    }
}