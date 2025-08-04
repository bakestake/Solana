using System;
using UnityEngine;

namespace Gamegaard
{
    [Serializable]
    public abstract class ObjectScrollPhaseValue
    {
        [Min(0)]
        [SerializeField] private float size = 0;
        [SerializeField] private Color color = Color.white;

        public ObjectScrollPhaseValue(float size, Color color)
        {
            this.size = size;
            this.color = color;
        }

        public float Size => size;
        public Color Color => color;
        public Vector2 Scale => new Vector2(size, size);

        public abstract void SetToInitialScale();
    }
}