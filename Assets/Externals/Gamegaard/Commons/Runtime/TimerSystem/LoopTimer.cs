namespace Gamegaard.Timer
{
    [System.Serializable]
    public class LoopTimer : Timer
    {
        #region Constructors
        public LoopTimer() { }
        public LoopTimer(float timerTime) : base(timerTime) { }
        public LoopTimer(float timerTime, float initialElapsedTime) : base(timerTime, initialElapsedTime) { }
        public LoopTimer(float timerTime, float initialElapsedTime, float timeScale) : base(timerTime, initialElapsedTime, timeScale) { }
        #endregion

        protected override void OnFinished()
        {
            Reset();
        }
    }
}