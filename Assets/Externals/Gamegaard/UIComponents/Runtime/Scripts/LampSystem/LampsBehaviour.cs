using UnityEngine;

namespace Gamegaard
{
    [System.Serializable]
    public class LampsBehaviour
    {
        public Color color = Color.white;
        public LampPattern pattern;
        public bool useMaxCycles;
        public int maxCycles;

        public int MaxCycles => useMaxCycles ? maxCycles : -1;
    }
}
