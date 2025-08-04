using UnityEngine;

namespace Gamegaard.FarmSystem
{
    [CreateAssetMenu(fileName = "Plant_", menuName = "Plants/Plant")]
    public class PlantData : ScriptableObject
    {
        [SerializeField] private string plantName;
        [SerializeField] private string plantDescription;

        [SerializeField] private PlantPhase[] phases;

        public string PlantName => plantName;
        public string PlantDescription => plantDescription;
        public PlantPhase[] Phases => phases;
    }
}