using UnityEngine;
using UnityEngine.Tilemaps;

namespace Gamegaard
{
    public class TilePlus : Tile
    {
        [Header("Attributes")]
        public TilePlus plow;
        public TilePlus water;
        public string[] fishs;

        public bool isPlantable;

        public bool IsArable => plow != null;
        public bool IsWaterable => water != null;

        public bool IsFishable => fishs.Length > 0;
    }
}
