using UnityEngine;

namespace Gamegaard.CustomValues
{
    [System.Serializable]
    public class SpriteValueWeight : WeightValue<Sprite>
    {
        public bool isHorizontalFlipAllowed;
        public bool isVerticalFlipAllowed;

        public SpriteValueWeight(Sprite obj, float weight = 1, bool isHorizontalFlipAllowed = false, bool isVerticalFlipAllowed = false) : base(obj, weight)
        {
            this.isHorizontalFlipAllowed = isHorizontalFlipAllowed;
            this.isVerticalFlipAllowed = isVerticalFlipAllowed;
        }
    }
}