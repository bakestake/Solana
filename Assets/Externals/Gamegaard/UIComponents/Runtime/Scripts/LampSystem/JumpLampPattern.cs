using UnityEngine;

namespace Gamegaard
{
    [System.Serializable]
    public struct JumpLampPattern
    {
        [Min(0)]
        public int lightsOn;
        [Min(0)]
        public int lightsOff;

        public int Lightcount => lightsOn + lightsOff;
        public bool IsValid => Lightcount > 0;

        public bool IsLightOn(int index)
        {
            int adjustedIndex = index % Lightcount;
            return adjustedIndex < lightsOn;
        }
    }
}
