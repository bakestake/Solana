using Gamegaard.Timer;
using UnityEngine;
using UnityEngine.Events;

namespace Gamegaard
{
    public class DelayedAction : MonoBehaviour
    {
        [SerializeField] private bool playOnAwake; 
        [SerializeField] private bool isUnscaledTime;
        [Min(0)]
        [SerializeField] private float lifeTime = 4;
        [SerializeField] private UnityEvent onTimeFinished;

        private BasicTimer lifeTimeTimer;

        private void Awake()
        {
            lifeTimeTimer = new BasicTimer(lifeTime);

            if (!playOnAwake)
            {
                lifeTimeTimer.Pause();
            }
        }

        private void Update()
        {
            if (isUnscaledTime)
            {
                lifeTimeTimer.UpdateTimer(Time.unscaledDeltaTime);
                if (lifeTimeTimer.IsCompleted)
                {
                    OnTimeEnd();
                }
            }
            else
            {
                if (lifeTimeTimer.CheckAndUpdateTimer())
                {
                    OnTimeEnd();
                }
            }
        }

        public void Trigger()
        {
            lifeTimeTimer.Restart();
        }

        private void OnTimeEnd()
        {
            onTimeFinished?.Invoke();
        }

        public void DestroyObject(GameObject gameObject)
        {
            Destroy(gameObject);
        }
    }
}