using UnityEngine;

namespace Gamegaard.Timer
{
    public class Waiter : IPauseable
    {
        private float pauseTime;
        private float targetTime = -1f;
        private float lastCheckTime;

        public float TargetTime
        {
            get
            {
                if (targetTime < 0)
                {
                    targetTime = Time.time + WaitTime;
                }
                return targetTime;
            }
        }
        public float WaitTime { get; private set; }
        public bool IsPaused { get; private set; }
        public bool IsCompleted => Time.time >= TargetTime;
        public bool IsRunning => !IsCompleted && !IsPaused;
        public float Progress => Mathf.Clamp01((TargetTime - Time.time) / WaitTime);
        public float InverseProgress => 1 - Mathf.Clamp01((TargetTime - Time.time) / WaitTime);
        public float ElapsedTime => Time.time - (TargetTime - WaitTime);
        public float RemainingTime => Mathf.Max(TargetTime - Time.time, 0);

        public Waiter()
        {

        }

        public Waiter(float waitTime)
        {
            WaitTime = waitTime;
        }

        public bool TryResetIfCompleted()
        {
            if (IsCompleted)
            {
                Reset();
                return true;
            }

            return false;
        }

        public bool IsCompletedThisFrame()
        {
            if (IsCompleted && lastCheckTime < TargetTime)
            {
                lastCheckTime = Time.time;
                return true;
            }
            return false;
        }

        public void SetAndReset(float waitTime)
        {
            WaitTime = waitTime;
            Reset();
        }

        public void SetWaitTime(float waitTime)
        {
            WaitTime = waitTime;
        }

        public void AddTime(float extraTime)
        {
            targetTime += extraTime;
        }

        public void Reset()
        {
            targetTime = Time.time + WaitTime;
        }

        public void Pause()
        {
            if (!IsPaused)
            {
                pauseTime = Time.time;
                IsPaused = true;
            }
        }

        public void Resume()
        {
            if (IsPaused)
            {
                float pausedDuration = Time.time - pauseTime;
                targetTime += pausedDuration;
                IsPaused = false;
            }
        }

        public void ForceComplete()
        {
            targetTime = Time.time;
        }

        public void SetElapsedPercentage(float percentage)
        {
            percentage = Mathf.Clamp01(percentage);
            targetTime = Time.time + WaitTime * (1 - percentage);
        }
    }
}