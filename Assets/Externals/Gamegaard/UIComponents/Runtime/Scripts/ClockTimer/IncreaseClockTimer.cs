
namespace Gamegaard
{
    public class IncreaseClockTimer : ClockTimer
    {
        public override float ElapsedTime
        {
            get => _elapsedTime;
            protected set => _elapsedTime = value;
        }

        protected override void UpdateTime()
        {
            ElapsedTime += GetElapsedTime();
        }
    }
}