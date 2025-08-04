using System.Collections;
using UnityEngine;

namespace Gamegaard
{
    public abstract class LampPattern : ScriptableObject
    {
        [Header("Pattern Configuration")]
        [SerializeField] protected float turnOnDelay = 0.5f;
        [SerializeField] protected float delayAfterComplete = 1f;
        [SerializeField] protected float restartDelay = 1f;
        [SerializeField] protected int maxOnLamps = 10;
        [SerializeField] protected int cycleOffset;

        public abstract IEnumerator ExecutePattern(ILamp[] lamps, int cyclesCount = -1, bool useUnscaledTime = false);

        protected IEnumerator WaitForSeconds(float timeInSeconds, bool useUnscaledTime)
        {
            float elapsedTime = 0f;

            while (elapsedTime < timeInSeconds)
            {
                elapsedTime += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }
        }
    }
}
