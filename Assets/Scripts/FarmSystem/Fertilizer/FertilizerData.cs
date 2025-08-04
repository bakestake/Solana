using UnityEngine;

namespace Gamegaard
{
    [CreateAssetMenu(menuName = "Plant/Fertilizer")]
    public class FertilizerData : ScriptableObject
    {
        [Min(0)]
        public float durationTime;
        [Min(0)]
        public float growUpMultiplier = 1;
        [Min(0)]
        public float qualityMultiplier = 1;
    }
}