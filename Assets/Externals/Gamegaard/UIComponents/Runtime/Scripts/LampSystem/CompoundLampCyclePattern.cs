using UnityEngine;

namespace Gamegaard
{
    [System.Serializable]
    public struct CompoundLampCyclePattern
    {
        public LampPattern pattern;
        [Min(1)]
        public int cyclesCount;

        public CompoundLampCyclePattern(LampPattern pattern, int cyclesCount)
        {
            this.pattern = pattern;
            this.cyclesCount = cyclesCount;
        }
    }
}