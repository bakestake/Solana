using Gamegaard.CustomValues;
using Gamegaard.Timer;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Gamegaard
{
    [Serializable]
    public class DelayedActionValue
    {
#if UNITY_EDITOR
        [SerializeField] private string eventName;
#endif
        [SerializeField] private bool isUnscaledTime;
        [SerializeField] private MinMaxFloat randomDelay;
        [SerializeField] private UnityEvent onTimeFinished;

        private BasicTimer lifeTimeTimer;
        private bool hasStarted;

        public void Initialize()
        {
            lifeTimeTimer = new BasicTimer(randomDelay.GetRandom());
        }

        public void Update()
        {
            if (!hasStarted) return;

            lifeTimeTimer.UpdateTimer(isUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime);

            if (lifeTimeTimer.IsCompleted)
            {
                OnTimeEnd();
            }
        }

        public void OnActionStart()
        {
            lifeTimeTimer.SetTimer(randomDelay.GetRandom());
            hasStarted = true;
        }

        public void OnActionEnd()
        {
            hasStarted = false;
        }

        public void SubscribeListner(Action listner)
        {
            onTimeFinished.AddListener(() => listner?.Invoke());
        }

        public void UnsubscribeListner(Action listner)
        {
            onTimeFinished.RemoveListener(() => listner?.Invoke());
        }

        public void ForceCall()
        {
            OnTimeEnd();
        }

        private void OnTimeEnd()
        {
            onTimeFinished?.Invoke();
        }
    }
}