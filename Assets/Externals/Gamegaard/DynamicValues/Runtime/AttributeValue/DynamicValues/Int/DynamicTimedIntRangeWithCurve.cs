using System;
using UnityEngine;

namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Extends <see cref="DynamicTimedIntRange"/> to include an animation curve, allowing the value shift to be dynamically adjusted based on a curve over time.
    /// </summary>
    [Serializable]
    public class DynamicTimedIntRangeWithCurve : DynamicTimedIntRange
    {
        [SerializeField]
        private AnimationCurve valueCurve;

        /// <summary>
        /// The curve that controls how the value shifts over time.
        /// </summary>
        public AnimationCurve ValueCurve => valueCurve;

        public DynamicTimedIntRangeWithCurve() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicTimedIntRangeWithCurve"/> class.
        /// </summary>
        /// <param name="baseValue">The base value for the range.</param>
        /// <param name="currentValue">The current value for the range.</param>
        /// <param name="changeBaseValue">The base value used for shifting the current value over time.</param>
        /// <param name="changeTime">The time interval at which the value should change.</param>
        /// <param name="valueCurve">The curve that will be used to adjust the value over time.</param>
        /// <param name="isPaused">Initial state of the value shift (paused or not).</param>
        public DynamicTimedIntRangeWithCurve(int baseValue, int currentValue, int changeBaseValue, float changeTime, AnimationCurve valueCurve, bool isPaused = false, float timeScale = 1)
            : base(baseValue, currentValue, changeBaseValue, changeTime, isPaused, timeScale)
        {
            this.valueCurve = valueCurve;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicTimedIntRangeWithCurve"/> class.
        /// <param name="other">Value to clone.</param>
        /// </summary>
        public DynamicTimedIntRangeWithCurve(DynamicTimedIntRangeWithCurve other) : this(other.BaseValue, other.CurrentValue, other.ValueShift.BaseValue, other.ChangeTime, other.ValueCurve)
        {

        }

        /// <summary>
        /// Updates the current value based on the timer and value curve.
        /// </summary>
        public override bool UpdateValueShift()
        {
            if (IsValueShiftPaused) return false;

            if (ValueShiftTimer.CheckAndUpdateTimer())
            {
                float curveTime = ValueShiftTimer.TimerDuration - ValueShiftTimer.RemainingTime;
                float curveValue = ValueCurve.Evaluate(curveTime / ValueShiftTimer.TimerDuration);
                CurrentValue += Mathf.RoundToInt(ValueShift.CalculatedValue * curveValue);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a string representation of the current state of the DynamicTimedIntRangeWithCurve.
        /// </summary>
        public override string ToString()
        {
            return $"{base.ToString()}, Curve Evaluation: {ValueCurve}";
        }
    }
}
