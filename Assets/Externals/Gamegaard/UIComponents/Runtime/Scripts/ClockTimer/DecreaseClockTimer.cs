using UnityEngine;
using UnityEngine.Events;

namespace Gamegaard
{
    public class DecreaseClockTimer : ClockTimer
    {
        [Min(0)]
        [Delayed]
        [SerializeField] private float initialTime;
        [SerializeField] private UnityEvent OnTimerEnd;

        private bool hasFinished;

        public bool IsCompleted => enabled && _elapsedTime <= 0;
        public float Percentage => _elapsedTime / initialTime;

        public override float ElapsedTime
        {
            get => _elapsedTime;
            protected set => _elapsedTime = Mathf.Max(value, 0);
        }

        protected override void OnValidate()
        {
            ResetTime();
            base.OnValidate();
        }

        protected virtual void Awake()
        {
            ResetTime();
        }

        protected override void UpdateTime()
        {
            ElapsedTime -= GetElapsedTime();
            if (ElapsedTime <= 0 && !hasFinished)
            {
                hasFinished = true;
                OnTimerEnd?.Invoke();
            }
        }

        public void ResetTime()
        {
            hasFinished = false;
            ElapsedTime = initialTime;
        }
    }
}