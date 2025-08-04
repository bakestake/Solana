using UnityEngine;
using Gamegaard.Timer;

namespace Game.UI.InLevelUpgrades
{
    public class TimedHoldOnObject : HoldOnObjectBase
    {
        [Min(0)]
        [SerializeField] private float holdOnTargetTime = 1;
        private BasicTimer timer;

        public float Percentage => timer.Progress;

        protected virtual void Awake()
        {
            timer = new BasicTimer(holdOnTargetTime);
        }

        protected override bool CheckCompletion()
        {
            return IsHolding && timer.CheckAndUpdateTimer();
        }

        public override void ResetValues()
        {
            base.ResetValues();
            timer.Reset();
        }
    }
}