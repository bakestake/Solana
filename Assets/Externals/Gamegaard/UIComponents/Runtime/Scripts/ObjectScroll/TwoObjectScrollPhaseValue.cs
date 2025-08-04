using System;
using UnityEngine;

namespace Gamegaard
{
    [Serializable]
    public class TwoObjectScrollPhaseValue<T> : ObjectScrollPhaseValue where T : ObjectScrollElement
    {
        public T previousElement;
        public T nextElement;

        public TwoObjectScrollPhaseValue(float size, Color color) : base(size, color)
        {
        }

        public override void SetToInitialScale()
        {
            previousElement.transform.localScale = Scale;
            nextElement.transform.localScale = Scale;
        }
    }
}