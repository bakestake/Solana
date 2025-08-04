using Gamegaard.Timer;
using System;
using UnityEngine;

namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Attribute that represents a float value that changes over time.
    /// Commonly used for attributes like Health, Mana, or Energy that regenerate or decay over time.
    /// </summary>
    [Serializable]
    public class DynamicTimedFloatRange : DynamicFloatRange
    {
        [SerializeField] private DynamicFloat valueShift;
        [SerializeField] private FreezeableLoopTimer valueShiftTimer;
        /// <summary>
        /// Represents the dynamic range that shifts the current value over time.
        /// </summary>
        public DynamicFloat ValueShift => valueShift;

        /// <summary>
        /// The timer responsible for controlling the interval for value shifts and handling freezing.
        /// </summary>
        public FreezeableLoopTimer ValueShiftTimer => valueShiftTimer;

        /// <summary>
        /// Indicates whether the value shift is paused.
        /// </summary>
        public bool IsValueShiftPaused { get; private set; }

        /// <summary>
        /// The time interval at which the value should shift.
        /// </summary>
        public float ChangeTime => ValueShiftTimer.TimerDuration;

        /// <summary>
        /// The time scale applied to the value shift timer.
        /// </summary>
        public float TimeScale => ValueShiftTimer.TimeScale;

        public DynamicTimedFloatRange() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicTimedFloatRange"/> class.
        /// </summary>
        /// <param name="baseValue">The base value for the range.</param>
        /// <param name="currentValue">The current value for the range.</param>
        /// <param name="changeBaseValue">The base value used for shifting the current value over time.</param>
        /// <param name="changeTime">The time interval at which the value should change.</param>
        /// <param name="isPaused">Initial state of the value shift (paused or not).</param>
        public DynamicTimedFloatRange(float baseValue, float currentValue, float changeBaseValue, float changeTime, bool isPaused = false, float timeScale = 1) : base(baseValue, currentValue)
        {
            IsValueShiftPaused = isPaused;
            valueShift = new DynamicFloatRange(changeBaseValue);
            valueShiftTimer = new FreezeableLoopTimer(changeTime, changeTime, timeScale);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicTimedFloatRange"/> class.
        /// </summary>
        /// <param name="baseValue">The base value for the range.</param>
        /// <param name="changeBaseValue">The base value used for shifting the current value over time.</param>
        /// <param name="changeTime">The time interval at which the value should change.</param>
        /// <param name="isPaused">Initial state of the value shift (paused or not).</param>
        public DynamicTimedFloatRange(float baseValue, float changeBaseValue, float changeTime, bool isPaused = false, float timeScale = 1) : this(baseValue, baseValue, changeBaseValue, changeTime, isPaused, timeScale)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicTimedFloatRange"/> class by copying another instance.
        /// </summary>
        /// <param name="other">Another instance of <see cref="DynamicTimedFloatRange"/> to copy values from.</param>
        public DynamicTimedFloatRange(DynamicTimedFloatRange other) : this(other.BaseValue, other.CurrentValue, other.ValueShift.BaseValue, other.ValueShiftTimer.TimerDuration, other.IsValueShiftPaused, other.ValueShiftTimer.TimeScale)
        {

        }

        /// <summary>
        /// Updates all dynamic behaviors of the range.
        /// This includes processing temporary modifiers and shifting the current value 
        /// based on the value shift timer.
        /// </summary>
        public virtual void UpdateAll()
        {
            UpdateTempModifiers();
            UpdateValueShift();
        }

        /// <summary>
        /// Updates the current value based on the timer and value shift.
        /// Returns true if the value was changed in this frame.
        /// </summary>
        public virtual bool UpdateValueShift()
        {
            if (IsValueShiftPaused) return false;

            if (ValueShiftTimer.CheckAndUpdateTimer())
            {
                CurrentValue += ValueShift.CalculatedValue;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Freezes the value change for a specified duration.
        /// </summary>
        /// <param name="timeInSeconds">The duration in seconds to freeze the value change.</param>
        public void Freeze(float timeInSeconds)
        {
            ValueShiftTimer.Freeze(timeInSeconds);
        }

        /// <summary>
        /// Unpauses the value shift, allowing the value to continue changing over time.
        /// </summary>
        public void Unfreeze()
        {
            ValueShiftTimer.Unfreeze();
        }

        /// <summary>
        /// Pauses the value shift, preventing any further changes until unpaused.
        /// </summary>
        public void Pause()
        {
            IsValueShiftPaused = true;
            ValueShiftTimer.Pause();
        }

        /// <summary>
        /// Instantly unfreezes the value change by forcing the frozen waiter to complete.
        /// </summary>
        public void Unpause()
        {
            IsValueShiftPaused = false;
            ValueShiftTimer.Resume();
        }

        /// <summary>
        /// Resets the interval for the value shift.
        /// </summary>
        public void ResetValueShiftTimer()
        {
            ValueShiftTimer.Reset();
        }

        /// <summary>
        /// Sets a new time scale for the timers.
        /// </summary>
        /// <param name="timeScale">The new timeScale.</param>
        public void SetTimeScale(float timeScale)
        {
            ValueShiftTimer.SetTimeScale(timeScale);
        }

        /// <summary>
        /// Returns a string representation of the current state of the DynamicTimedFloatRange.
        /// Includes details such as current value, base value, value shift, time scale, change time, and pause/frozen states.
        /// </summary>
        /// <returns>A string that represents the current state of the DynamicTimedFloatRange.</returns>
        public override string ToString()
        {
            return $"Current Value: {CurrentValue}, Base Value: {BaseValue}, Value Shift: {ValueShift.CalculatedValue}, TimeScale: {TimeScale}, Change Time: {ChangeTime}, IsPaused: {IsValueShiftPaused}, IsFrozen: {!ValueShiftTimer.FrozenTimer.IsCompleted}";
        }
    }
}