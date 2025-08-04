using System.Collections;
using UnityEngine;

namespace Gamegaard
{
    public class FreezeController
    {
        private readonly TimeController timeController;
        private readonly PauseSystem pauseSystem;
        private Coroutine freezeTimeCoroutine;
        private bool isFrozen;
        private float lastFreezeTimeScale;

        /// <summary>
        /// Gets a value indicating whether the game is currently frozen.
        /// </summary>
        public bool IsFrozen => isFrozen;

        public FreezeController(PauseSystem pauseSystem, TimeController timeController)
        {
            this.pauseSystem = pauseSystem;
            this.timeController = timeController;
        }

        /// <summary>
        /// Freezes the game for a specified duration.
        /// </summary>
        /// <param name="timeInSeconds">The duration to freeze the game in seconds.</param>
        public void FreezeTime(float timeInSeconds)
        {
            if (timeController.IsPaused) return;

            if (freezeTimeCoroutine != null)
            {
                pauseSystem.StopCoroutine(freezeTimeCoroutine);
            }
            freezeTimeCoroutine = pauseSystem.StartCoroutine(ResumeAfterDelay(timeInSeconds));
        }

        /// <summary>
        /// Unfreezes the game, if it was previously frozen.
        /// </summary>
        public void UnfreezeTime()
        {
            if (!isFrozen) return;

            if (freezeTimeCoroutine != null)
            {
                pauseSystem.StopCoroutine(freezeTimeCoroutine);
            }
            isFrozen = false;
            Time.timeScale = lastFreezeTimeScale;
        }

        private IEnumerator ResumeAfterDelay(float timeInSeconds)
        {
            isFrozen = true;
            lastFreezeTimeScale = Time.timeScale;
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(timeInSeconds);
            Time.timeScale = lastFreezeTimeScale;
            isFrozen = false;
        }
    }
}