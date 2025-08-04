using Gamegaard.Timer;

namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Represents a temporary modifier that expires after a certain duration.
    /// </summary>
    /// <typeparam name="TValue">The type of value being modified, constrained to numeric-compatible types.</typeparam>
    public abstract class TemporaryModifier<TValue> : Modifier<TValue>, IPauseable
    {
        /// <summary>
        /// The timer that tracks the duration of the modifier.
        /// </summary>
        private readonly BasicTimer durationTimer;

        /// <summary>
        /// The total duration of the modifier in seconds.
        /// </summary>
        public readonly float durationInSeconds;

        /// <summary>
        /// Gets a value indicating whether the modifier's duration has completed.
        /// </summary>
        public bool HasFinished => durationTimer.IsCompleted;

        /// <summary>
        /// Gets the progress of the timer as a percentage (0 to 1).
        /// </summary>
        public float Percentage => durationTimer.Progress;

        /// <summary>
        /// Gets the total duration of the modifier's timer in seconds.
        /// </summary>
        public float DurationInSeconds => durationTimer.TimerDuration;

        public bool IsPaused => durationTimer.IsPaused;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporaryModifier{T}"/> class.
        /// </summary>
        /// <param name="type">The calculation type of the modifier.</param>
        /// <param name="value">The value of the modifier.</param>
        /// <param name="durationInSeconds">The duration of the modifier in seconds.</param>
        /// <param name="source">The source object associated with this modifier.</param>
        /// <param name="calculationStrategy">The calculation strategy to apply (optional).</param>
        public TemporaryModifier(ModifierCalculationType type, TValue value, float durationInSeconds, object source, string id = null, ModifierStrategy<TValue> calculationStrategy = null) : base(type, value, source, id, calculationStrategy)
        {
            this.durationInSeconds = durationInSeconds;
            durationTimer = new BasicTimer(durationInSeconds);
        }

        public TemporaryModifier(TemporaryModifier<TValue> other) : this(other.type, other.baseValue, other.durationInSeconds, other.source, other.id, other.calculationStrategy) 
        {
        }

        /// <summary>
        /// Updates the modifier's timer.
        /// </summary>
        public virtual void Update()
        {
            durationTimer.UpdateTimer();
        }

        /// <summary>
        /// Updates the modifier's timer and returns whether the duration has completed.
        /// </summary>
        /// <returns>True if the timer has completed, otherwise false.</returns>
        public virtual bool CheckedUpdate()
        {
            return durationTimer.CheckAndUpdateTimer();
        }

        public void Pause()
        {
            durationTimer.Pause();
        }

        public void Resume()
        {
            durationTimer.Resume();
        }

        /// <summary>
        /// Resets the timer to its initial state with zero elapsed time.
        /// </summary>
        public void ResetTimer()
        {
            durationTimer.Reset();
        }

        /// <summary>
        /// Sets the timer with the specified duration and resets the current time to zero.
        /// </summary>
        /// <param name="timerDuration">The total duration of the timer.</param>
        public void SetTimerTime(float timerDuration)
        {
            durationTimer.SetTimer(timerDuration);
        }
    }
}