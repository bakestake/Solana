using Gamegaard.Timer;
using UnityEngine;
using UnityEngine.Events;

namespace Bakeland
{
    public class LoopTimedAction : MonoBehaviour
    {
        [SerializeField] private float time;
        [SerializeField] private float initialElapsedtime;
        [SerializeField] private bool randomizeInitialValue;
        [SerializeField] private UnityEvent OnTimeEnd;

        private BasicTimer timer;

        private void Awake()
        {
            float initialValue = randomizeInitialValue ? Random.Range(0, time) : time;
            timer = new BasicTimer(initialValue, initialElapsedtime);
        }

        private void Update()
        {
            if (timer.CheckAndUpdateTimer())
            {
                OnTimeEnd?.Invoke();
                ResetTimer();
            }
        }

        public void ResetTimer()
        {
            timer.SetTimer(time);
        }
    }
}