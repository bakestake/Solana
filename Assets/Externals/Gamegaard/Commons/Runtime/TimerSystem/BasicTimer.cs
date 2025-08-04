namespace Gamegaard.Timer
{
    [System.Serializable]
    public class BasicTimer : Timer
    {
        protected bool hasFinished;

        public BasicTimer() { }
        public BasicTimer(float timerDuration) : base(timerDuration) { }
        public BasicTimer(float timerDuration, float initialElapsedTime) : base(timerDuration, initialElapsedTime) { }
        public BasicTimer(float timerDuration, float initialElapsedTime, float timeScale) : base(timerDuration, initialElapsedTime, timeScale) { }

        protected override void OnFinished()
        {
            _elapsedTime = TimerDuration;
            hasFinished = true;
        }

        protected override bool CanUpdate()
        {
            return base.CanUpdate() && !hasFinished;
        }

        public override void SetTimer(float timerTime)
        {
            base.SetTimer(timerTime);
            hasFinished = false;
        }

        public override void SetTimer(float timerTime, float elapsedTime)
        {
            base.SetTimer(timerTime, elapsedTime);
            hasFinished = false;
        }

        public override void Reset()
        {
            base.Reset();
            hasFinished = false;
        }

        public override void Restart()
        {
            base.Restart();
            hasFinished = false;
        }

        public void Finish()
        {
            OnFinished();
        }
    }
}