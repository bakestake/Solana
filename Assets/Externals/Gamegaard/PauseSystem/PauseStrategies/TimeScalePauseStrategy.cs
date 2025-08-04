using UnityEngine;

namespace Gamegaard
{
    public class TimeScalePauseStrategy : IPauseStrategy
    {
        private float lastTimeScale;

        public void Pause(GameObject caster)
        {
            lastTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }

        public void Unpause(GameObject caster)
        {
            Time.timeScale = lastTimeScale;
        }
    }
}
