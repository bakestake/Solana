using System;
using UnityEngine;

namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Represents an integer range with an associated animation curve, allowing dynamic value adjustments based on a percentage over time.
    /// </summary>
    [Serializable]
    public class IntRangeWithCurve : IntRange
    {
        /// <summary>
        /// The curve that controls how the value is set based on the percentage over time.
        /// </summary>
        public AnimationCurve ValueCurve { get; private set; }

        public IntRangeWithCurve() : base() { }

        public IntRangeWithCurve(int maxValue, int currentValue, int minValue, AnimationCurve valueCurve) : base(maxValue, currentValue, minValue)
        {
            ValueCurve = valueCurve;
        }

        public IntRangeWithCurve(IntRangeWithCurve other) : base(other)
        {
            ValueCurve = other.ValueCurve;
        }

        /// <summary>
        /// Sets the current value based on a percentage, modified by the animation curve.
        /// </summary>
        public override void SetToPercentage(float percentage)
        {
            float curveValue = ValueCurve.Evaluate(percentage);
            CurrentValue = Mathf.RoundToInt(_minValue + ((_maxValue - _minValue) * curveValue));
        }

        /// <summary>
        /// Returns a string representation of the current state of the IntRangeWithCurve.
        /// </summary>
        public override string ToString()
        {
            return $"{base.ToString()}, Curve Evaluation: {ValueCurve}";
        }
    }
}