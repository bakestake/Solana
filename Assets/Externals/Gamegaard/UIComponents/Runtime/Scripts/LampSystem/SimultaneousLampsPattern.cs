using System.Collections;
using UnityEngine;

namespace Gamegaard
{
    [CreateAssetMenu(fileName = "SimultaneousLampsPattern_", menuName = "LampPatterns/AlternatingSimultaneous")]
    public class SimultaneousLampsPattern : LampPattern
    {
        [SerializeField] private JumpLampPattern[] steps;

        public override IEnumerator ExecutePattern(ILamp[] lamps, int cyclesCount = -1, bool useUnscaledTime = false)
        {
            int currentCycle = 0;
            StepTracker patternSteps = new StepTracker(steps.Length, true);

            while (cyclesCount == -1 || currentCycle < cyclesCount)
            {
                bool shouldActivateAll = steps.Length == 0;

                if (shouldActivateAll)
                {
                    ActivateAllLamps(lamps);
                }
                else if (!steps[patternSteps.CurrentStepIndex].IsValid)
                {
                    DeactivateAllLamps(lamps);
                }
                else
                {
                    ExecuteStepLight(steps[patternSteps.CurrentStepIndex], lamps, currentCycle);
                }

                yield return WaitForSeconds(delayAfterComplete, useUnscaledTime);

                DeactivateAllLamps(lamps);

                yield return WaitForSeconds(restartDelay, useUnscaledTime);

                patternSteps.NextStep();
                currentCycle++;
            }
        }

        private void DeactivateAllLamps(ILamp[] lamps)
        {
            foreach (ILamp lamp in lamps)
            {
                lamp.SetActiveState(false);
            }
        }

        private void ActivateAllLamps(ILamp[] lamps)
        {
            foreach (ILamp lamp in lamps)
            {
                lamp.SetActiveState(true);
            }
        }

        private void ExecuteStepLight(JumpLampPattern step, ILamp[] lamps, int currentCycle)
        {
            for (int i = 0; i < lamps.Length; i++)
            {
                ILamp lamp = lamps[i];
                lamp.SetActiveState(step.IsLightOn(i + (currentCycle * cycleOffset)));
            }
        }
    }
}