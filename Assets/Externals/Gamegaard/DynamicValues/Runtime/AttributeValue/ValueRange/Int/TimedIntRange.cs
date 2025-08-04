using Gamegaard.Timer;
using System;
using UnityEngine;

namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Represents an integer range with timed value shifts, allowing for automatic value adjustments over time.
    /// </summary>
    [Serializable]
    public class TimedIntRange : IntRange
    {
        [SerializeField] protected FreezeableLoopTimer valueShiftTimer;
        [SerializeField] protected int valueShift;

        /// <summary>
        /// The timer controlling the interval for value shifts.
        /// </summary>
        public FreezeableLoopTimer ValueShiftTimer => valueShiftTimer;

        /// <summary>
        /// Defines the range and magnitude of value shifts applied over time.
        /// </summary>
        public int ValueShift => valueShift;

        /// <summary>
        /// Indicates whether the value shift process is currently paused.
        /// </summary>
        public bool IsValueShiftPaused { get; private set; }

        /// <summary>
        /// Gets the duration of each value shift cycle.
        /// </summary>
        public float ChangeTime => ValueShiftTimer.TimerDuration;

        /// <summary>
        /// Gets the current time scale applied to the timer.
        /// </summary>
        public float TimeScale => ValueShiftTimer.TimeScale;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedIntRange"/> class with default values.
        /// </summary>
        public TimedIntRange() : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedIntRange"/> class with specified parameters.
        /// </summary>
        /// <param name="maxValue">The maximum value of the range.</param>
        /// <param name="currentValue">The current value of the range.</param>
        /// <param name="valueShift">The base value for value shifts.</param>
        /// <param name="changeTime">The duration for each value shift.</param>
        /// <param name="isPaused">Indicates whether the value shift process starts paused.</param>
        /// <param name="timeScale">The time scale for the timer.</param>
        public TimedIntRange(int maxValue, int currentValue, int valueShift, float changeTime, bool isPaused = false, float timeScale = 1) : base(maxValue, currentValue)
        {
            this.valueShift = valueShift;
            valueShiftTimer = new FreezeableLoopTimer(changeTime, changeTime, timeScale);
            IsValueShiftPaused = isPaused;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedIntRange"/> class by copying values from another instance.
        /// </summary>
        /// <param name="other">The instance to copy values from.</param>
        public TimedIntRange(TimedIntRange other) : this(other.MaxValue, other.CurrentValue, other.valueShift, other.ChangeTime, other.IsValueShiftPaused, other.TimeScale) { }

        /// <summary>
        /// Updates the current value by applying the value shift if the timer has completed.
        /// </summary>
        /// <returns>True if the value was updated; otherwise, false.</returns>
        public bool UpdateChangedValue()
        {
            if (IsValueShiftPaused) return false;

            if (ValueShiftTimer.CheckAndUpdateTimer())
            {
                CurrentValue += valueShift;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Freezes the timer for a specified duration.
        /// </summary>
        /// <param name="timeInSeconds">The duration to freeze the timer, in seconds.</param>
        public void Freeze(float timeInSeconds)
        {
            ValueShiftTimer.Freeze(timeInSeconds);
        }

        /// <summary>
        /// Unfreezes the timer, resuming its progress.
        /// </summary>
        public void Unfreeze()
        {
            ValueShiftTimer.Unfreeze();
        }

        /// <summary>
        /// Pauses the value shift process and the associated timer.
        /// </summary>
        public void Pause()
        {
            IsValueShiftPaused = true;
            ValueShiftTimer.Pause();
        }

        /// <summary>
        /// Resumes the value shift process and the associated timer.
        /// </summary>
        public void Unpause()
        {
            IsValueShiftPaused = false;
            ValueShiftTimer.Resume();
        }

        /// <summary>
        /// Sets the time scale for the timer, affecting its speed.
        /// </summary>
        /// <param name="timeScale">The new time scale value.</param>
        public void SetTimeScale(float timeScale)
        {
            ValueShiftTimer.SetTimeScale(timeScale);
        }
    }
}