using UnityEngine;

namespace Gamegaard.Timer
{
    [System.Serializable]
    public abstract class Timer : IPauseable
    {
        [SerializeField] private float timeScale = 1;
        [SerializeField] private float timerDuration;

        protected float _elapsedTime;
        protected float _previousElapsedTime;

        /// <summary>
        /// Indicates if the timer is currently running (not completed or paused).
        /// </summary>
        public bool IsRunning => !IsCompleted && !IsPaused;

        /// <summary>
        /// Returns true if the timer has finished (current time reached or exceeded maximum time).
        /// </summary>
        public bool IsCompleted => _elapsedTime >= timerDuration;

        /// <summary>
        /// The time scale applied to the timer, affecting its speed.
        /// </summary>
        public float TimeScale => timeScale;

        /// <summary>
        /// The total duration of the timer.
        /// </summary>
        public float TimerDuration => timerDuration;

        /// <summary>
        /// The remaining time before the timer is completed.
        /// </summary>
        public float RemainingTime => timerDuration - _elapsedTime;

        /// <summary>
        /// The time that has passed since the timer started.
        /// </summary>
        public float ElapsedTime => _elapsedTime;

        /// <summary>
        /// Returns a value between 0 and 1 indicating the progress of the timer (1 means completed).
        /// </summary>
        public float Progress => Mathf.Clamp01(_elapsedTime / timerDuration);

        /// <summary>
        /// Returns a value between 0 and 1 indicating the inverse progress of the timer (1 means just started).
        /// </summary>
        public float InverseProgress => 1 - Mathf.Clamp01(_elapsedTime / timerDuration);

        /// <summary>
        /// Indicates if the timer is currently paused.
        /// </summary>
        public bool IsPaused { get; private set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Timer() { }

        /// <summary>
        /// Initializes a new timer with the specified duration.
        /// </summary>
        /// <param name="timerDuration">The total duration of the timer.</param>
        public Timer(float timerDuration)
        {
            SetTimer(timerDuration);
        }

        /// <summary>
        /// Initializes a new timer with the specified duration and current time.
        /// </summary>
        /// <param name="timerDuration">The total duration of the timer.</param>
        /// <param name="initialElapsedTime">The initial elapsed time of the timer.</param>
        public Timer(float timerDuration, float initialElapsedTime)
        {
            this.timerDuration = timerDuration;
            _elapsedTime = initialElapsedTime;
        }

        /// <summary>
        /// Initializes a new timer with the specified duration, current time, and time scale.
        /// </summary>
        /// <param name="timerDuration">The total duration of the timer.</param>
        /// <param name="initialElapsedTime">The initial elapsed time of the timer.</param>
        /// <param name="timeScale">The time scale affecting the speed of the timer.</param>
        public Timer(float timerDuration, float initialElapsedTime, float timeScale) : this(timerDuration, initialElapsedTime)
        {
            this.timeScale = timeScale;
        }

        /// <summary>
        /// Updates the timer using Time.deltaTime. If the timer is completed, calls OnFinished().
        /// </summary>
        public virtual void UpdateTimer()
        {
            _previousElapsedTime = _elapsedTime;
            if (!CanUpdate()) return;

            _elapsedTime += Time.deltaTime * timeScale;
            if (_elapsedTime >= timerDuration) OnFinished();
        }

        /// <summary>
        /// Updates the timer and checks if it is completed. If completed, returns true and calls OnFinished().
        /// </summary>
        /// <returns>True if the timer is completed, otherwise false.</returns>
        public virtual bool CheckAndUpdateTimer()
        {
            _previousElapsedTime = _elapsedTime;
            if (!CanUpdate()) return false;

            _elapsedTime += Time.deltaTime * timeScale;
            if (_elapsedTime >= timerDuration)
            {
                OnFinished();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Updates the timer using a custom elapsed time value. If the timer is completed, calls OnFinished().
        /// </summary>
        /// <param name="elapsedTime">The custom elapsed time to update the timer.</param>
        public virtual void UpdateTimer(float elapsedTime)
        {
            _previousElapsedTime = _elapsedTime;
            if (!CanUpdate()) return;

            _elapsedTime += elapsedTime * timeScale;
            if (_elapsedTime >= timerDuration) OnFinished();
        }

        /// <summary>
        /// Updates the timer with a custom elapsed time and checks if it is completed. If completed, returns true and calls OnFinished().
        /// </summary>
        /// <param name="elapsedTime">The custom elapsed time to update the timer.</param>
        /// <returns>True if the timer is completed, otherwise false.</returns>
        public virtual bool CheckAndUpdateTimer(float elapsedTime)
        {
            _previousElapsedTime = _elapsedTime;
            if (!CanUpdate()) return false;

            _elapsedTime += elapsedTime * timeScale;
            if (_elapsedTime >= timerDuration)
            {
                OnFinished();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Abstract method that is called when the timer completes. Must be implemented by subclasses.
        /// </summary>
        protected abstract void OnFinished();

        /// <summary>
        /// Checks if the elapsed time has changed since the last frame.
        /// </summary>
        /// <returns>True if the elapsed time has changed; otherwise, false.</returns>
        public bool HasElapsedTimeChanged()
        {
            return !Mathf.Approximately(_elapsedTime, _previousElapsedTime);
        }

        /// <summary>
        /// Pauses the timer, preventing further updates.
        /// </summary>
        public virtual void Pause()
        {
            IsPaused = true;
        }

        /// <summary>
        /// Resumes the timer after being paused.
        /// </summary>
        public virtual void Resume()
        {
            IsPaused = false;
        }

        /// <summary>
        /// Adds additional time to the timer.
        /// </summary>
        /// <param name="value">The amount of time to add to the timer.</param>
        public void AddCurrentTime(float value)
        {
            _elapsedTime += value;
        }

        /// <summary>
        /// Sets the timer with the specified duration and resets the current time to zero.
        /// </summary>
        /// <param name="timerDuration">The total duration of the timer.</param>
        public virtual void SetTimer(float timerDuration)
        {
            this.timerDuration = timerDuration;
            _elapsedTime = 0;
        }

        /// <summary>
        /// Sets the timer with the specified duration and current time.
        /// </summary>
        /// <param name="timerDuration">The total duration of the timer.</param>
        /// <param name="elapsedTime">The current time of the timer.</param>
        public virtual void SetTimer(float timerDuration, float elapsedTime)
        {
            this.timerDuration = timerDuration;
            _elapsedTime = elapsedTime;
        }

        /// <summary>
        /// Resets the timer to its initial state with zero elapsed time.
        /// </summary>
        public virtual void Reset()
        {
            _elapsedTime = 0;
        }

        /// <summary>
        /// Restarts the timer, setting elapsed time back to zero while keeping the current duration and time scale.
        /// </summary>
        public virtual void Restart()
        {
            _elapsedTime = 0;
            IsPaused = false;
        }

        /// <summary>
        /// Sets the time scale for the timer, which affects its speed.
        /// </summary>
        /// <param name="timeScale">The time scale to set.</param>
        public void SetTimeScale(float timeScale)
        {
            this.timeScale = timeScale;
        }

        /// <summary>
        /// Extends the timer's duration by a specified amount.
        /// </summary>
        /// <param name="extraTime">The amount of time to add to the timer's duration.</param>
        public void ExtendDuration(float extraTime)
        {
            timerDuration += extraTime;
        }

        /// <summary>
        /// Reduces the timer's duration by a specified amount, ensuring it doesn't go below zero.
        /// </summary>
        /// <param name="timeReduction">The amount of time to subtract from the timer's duration.</param>
        public void ReduceDuration(float timeReduction)
        {
            timerDuration = Mathf.Max(0, timerDuration - timeReduction);
        }

        /// <summary>
        /// Sets the elapsed time to a specific value, clamping it within the timer's range.
        /// </summary>
        /// <param name="time">The elapsed time to set.</param>
        public void SetElapsedTime(float time)
        {
            _elapsedTime = Mathf.Clamp(time, 0, timerDuration);
        }

        /// <summary>
        /// Sets the progress of the timer directly by adjusting the elapsed time.
        /// </summary>
        /// <param name="progress">The progress value to set, between 0 and 1.</param>
        public void SetProgress(float progress)
        {
            _elapsedTime = Mathf.Clamp01(progress) * timerDuration;
        }

        /// <summary>
        /// Determines if the timer can be updated (i.e., is not paused).
        /// </summary>
        /// <returns>True if the timer can be updated, otherwise false.</returns>
        protected virtual bool CanUpdate()
        {
            return !IsPaused;
        }

        /// <summary>
        /// Calculates the number of full timer cycles that would complete if the specified time in seconds passed,
        /// optionally taking into account the current elapsed time on the timer.
        /// </summary>
        /// <param name="seconds">The amount of time in seconds to simulate.</param>
        /// <param name="useElapsedTime">If true, includes the current elapsed time in the calculation; otherwise, calculates as if starting from zero.</param>
        /// <returns>The number of full timer cycles that would complete.</returns>
        public virtual int GetEstimatedFullCycles(float seconds, bool useElapsedTime = true)
        {
            return Mathf.FloorToInt(GetEstimatedCycles(seconds, useElapsedTime));
        }

        /// <summary>
        /// Calculates the number of timer cycles, including fractional completions, that would occur if the specified time in seconds passed,
        /// optionally including the current elapsed time on the timer.
        /// </summary>
        /// <param name="seconds">The amount of time in seconds to simulate.</param>
        /// <param name="useElapsedTime">If true, includes the current elapsed time in the calculation; otherwise, calculates as if starting from zero.</param>
        /// <returns>The number of timer cycles, including fractional values, that would complete.</returns>
        public virtual float GetEstimatedCycles(float seconds, bool useElapsedTime = true)
        {
            if (timerDuration <= 0) return 0f;

            float adjustedTime = seconds * timeScale;

            if (useElapsedTime)
            {
                adjustedTime += _elapsedTime;
            }

            return adjustedTime / timerDuration;
        }

        /// <summary>
        /// Returns the elapsed or remaining time as a percentage of the total duration.
        /// </summary>
        /// <param name="useInverse">If true, returns current percentage; otherwise, returns the remaining time percentage.</param>
        /// <returns>A formatted string representing the percentage.</returns>
        public string GetTimeAsPercentage(bool useInverse = true)
        {
            float percentage = useInverse ? Progress * 100 : InverseProgress * 100;
            return $"{percentage:0.00}%";
        }

        /// <summary>
        /// Calculates the elapsed time needed to reach a specific progress point.
        /// </summary>
        /// <param name="progress">The target progress value (between 0 and 1).</param>
        /// <returns>The corresponding elapsed time.</returns>
        public float CalculateTimeAtProgress(float progress)
        {
            return Mathf.Clamp01(progress) * timerDuration;
        }

        /// <summary>
        /// Checks if the elapsed time is within a specified range.
        /// </summary>
        /// <param name="start">The start of the time range.</param>
        /// <param name="end">The end of the time range.</param>
        /// <returns>True if the elapsed time falls within the range, otherwise false.</returns>
        public bool IsWithinTimeRange(float start, float end)
        {
            return _elapsedTime >= start && _elapsedTime <= end;
        }

        /// <summary>
        /// Checks if the elapsed time has surpassed a specific value.
        /// </summary>
        /// <param name="time">The time in seconds to check against.</param>
        /// <returns>True if elapsed time has surpassed the specified value, otherwise false.</returns>
        public bool HasPassedTime(float time)
        {
            return _elapsedTime >= time;
        }

        /// <summary>
        /// Toggles the pause state of the timer.
        /// </summary>
        public void TogglePause()
        {
            if (IsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
}