using System;

namespace Gamegaard.FarmSystem
{
    [Serializable]
    public struct PlantGrowUpSaveData
    {
        public int phaseIndex;
        public float elapsedTime;
        public string lastSaveTime;
    }
}