using UnityEngine;

namespace Gamegaard
{
    public interface IPauseStrategy
    {
        /// <summary>
        /// Pauses the game or the associated action, implementing the specific pause strategy.
        /// The object calling the method is passed as a parameter to customize the pause behavior.
        /// </summary>
        /// <param name="caster">The object that invoked the pause. It can be used to determine specific pause behavior.</param>
        void Pause(GameObject caster);

        /// <summary>
        /// Resumes the game or the associated action, reversing the effects of the pause.
        /// The object calling the method is passed as a parameter to customize the unpause behavior.
        /// </summary>
        /// <param name="caster">The object that invoked the unpause. It can be used to determine specific unpause behavior.</param>
        void Unpause(GameObject caster);
    }
}
