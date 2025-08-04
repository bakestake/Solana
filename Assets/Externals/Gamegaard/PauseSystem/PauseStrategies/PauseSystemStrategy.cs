using UnityEngine;

namespace Gamegaard
{
    public class PauseSystemStrategy : IPauseStrategy
    {
        private readonly bool isPausedByCaster;

        public PauseSystemStrategy(bool isPausedByCaster)
        {
            this.isPausedByCaster = isPausedByCaster;
        }

        public void Pause(GameObject caster)
        {
            if (isPausedByCaster)
            {
                PauseSystem.Instance.PauseByCaster(caster);
            }
            else
            {
                PauseSystem.Instance.Pause();
            }
        }

        public void Unpause(GameObject caster)
        {
            if (isPausedByCaster)
            {
                PauseSystem.Instance.UnpauseByCaster(caster);
            }
            else
            {
                PauseSystem.Instance.Unpause();
            }
        }
    }
}
