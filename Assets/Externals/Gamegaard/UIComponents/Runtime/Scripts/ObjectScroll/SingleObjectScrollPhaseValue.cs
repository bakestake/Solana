using System;
using UnityEngine;

namespace Gamegaard
{
    [Serializable]
    public class SingleObjectScrollPhaseValue<T> : ObjectScrollPhaseValue where T : ObjectScrollElement
    {
        public T element;

        public SingleObjectScrollPhaseValue(float size, Color color) : base(size, color)
        {
        }

        public override void SetToInitialScale()
        {
            element.transform.localScale = Scale;
        }
    }
}