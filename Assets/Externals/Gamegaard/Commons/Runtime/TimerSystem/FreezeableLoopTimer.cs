namespace Gamegaard.Timer
{
    [System.Serializable]
    public class FreezeableLoopTimer : LoopTimer
    {
        public BasicTimer FrozenTimer { get; private set; }

        public FreezeableLoopTimer()
        {
            FrozenTimer = new BasicTimer();
        }

        public FreezeableLoopTimer(float timerTime) : base(timerTime)
        {
            FrozenTimer = new BasicTimer();
        }

        public FreezeableLoopTimer(float timerTime, float actualTime) : base(timerTime, actualTime)
        {
            FrozenTimer = new BasicTimer();
        }

        public FreezeableLoopTimer(float timerTime, float actualTime, float timeScale) : base(timerTime, actualTime, timeScale)
        {
            FrozenTimer = new BasicTimer();
        }

        protected override bool CanUpdate()
        {
            FrozenTimer.UpdateTimer();
            return FrozenTimer.IsCompleted;
        }

        public void Freeze(float timeInSeconds)
        {
            FrozenTimer.SetTimer(timeInSeconds);
        }

        public void Unfreeze()
        {
            FrozenTimer.Finish();
        }

        public override void Pause()
        {
            base.Pause();
            FrozenTimer.Pause();
        }

        public override void Resume()
        {
            base.Resume();
            FrozenTimer.Resume();
        }
    }
}