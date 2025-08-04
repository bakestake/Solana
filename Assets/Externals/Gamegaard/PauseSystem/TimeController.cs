using System;
using UnityEngine;

namespace Gamegaard
{
    public class TimeController
    {
        public event Action OnGamePaused;
        public event Action OnGameResumed;
        private float lastTimeScale;

        public bool IsPaused => Time.timeScale == 0;

        public TimeController()
        {
            lastTimeScale = Time.timeScale;
        }

        public void CheckForTimeScaleChange()
        {
            if (Time.timeScale != lastTimeScale)
            {
                if (Time.timeScale == 0f)
                {
                    OnGamePaused?.Invoke();
                }
                else
                {
                    OnGameResumed?.Invoke();
                }

                lastTimeScale = Time.timeScale;
            }
        }

        /// <summary>
        /// Pauses the game.
        /// </summary>
        public void Pause()
        {
            lastTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }

        /// <summary>
        /// Unpauses the game.
        /// </summary>
        public void Unpause()
        {
            Time.timeScale = lastTimeScale;
        }
    }
}