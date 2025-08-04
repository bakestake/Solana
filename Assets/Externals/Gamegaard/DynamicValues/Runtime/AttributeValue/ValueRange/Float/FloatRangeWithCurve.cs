using System;
using UnityEngine;

namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Represents a floating-point range with an associated animation curve, allowing dynamic value adjustments based on a percentage over time.
    /// </summary>
    [Serializable]
    public class FloatRangeWithCurve : FloatRange
    {
        [SerializeField] protected AnimationCurve valueCurve;

        /// <summary>
        /// The curve that controls how the value is set based on the percentage over time.
        /// </summary>
        public AnimationCurve ValueCurve => valueCurve;

        public FloatRangeWithCurve() : base() { }

        public FloatRangeWithCurve(float maxValue, float currentValue, float minValue, AnimationCurve valueCurve) : base(maxValue, currentValue, minValue)
        {
            this.valueCurve = valueCurve;
        }

        public FloatRangeWithCurve(FloatRangeWithCurve other) : base(other)
        {
            valueCurve = other.ValueCurve;
        }

        /// <summary>
        /// Sets the current value based on a percentage, modified by the animation curve.
        /// </summary>
        public override void SetToPercentage(float percentage)
        {
            float curveValue = ValueCurve.Evaluate(percentage);
            CurrentValue = _minValue + ((_maxValue - _minValue) * curveValue);
        }

        /// <summary>
        /// Returns a string representation of the current state of the FloatRangeWithCurve.
        /// </summary>
        public override string ToString()
        {
            return $"{base.ToString()}, Curve Evaluation: {ValueCurve}";
        }
    }
}