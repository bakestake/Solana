using System.Collections;
using UnityEngine;

namespace Gamegaard
{
    [CreateAssetMenu(fileName = "TimedLampPattern_", menuName = "LampPatterns/TimedLampPattern")]
    public class TimedLampPattern : LampPattern
    {
        [SerializeField] private JumpLampPattern[] steps;
        [SerializeField] private bool invertStart = false;
        [SerializeField] private bool zigzag = true;

        public override IEnumerator ExecutePattern(ILamp[] lamps, int cyclesCount = -1, bool useUnscaledTime = false)
        {
            bool isInverse = invertStart;
            int maxOnLamps = Mathf.Min(lamps.Length, this.maxOnLamps);
            int usedLamps = 0;
            int currentCycle = 0;
            bool resetThisFrame = false;
            StepTracker patternSteps = new StepTracker(steps.Length, true);
            StepTracker lampsSteps = new StepTracker(lamps.Length, false, isInverse ? lamps.Length - 1 : 0);

            while (cyclesCount == -1 || currentCycle < cyclesCount)
            {
                if (steps.Length == 0)
                {
                    yield return ExecuteDefaultLight();
                }
                else
                {
                    while (CheckWhileCondition())
                    {
                        JumpLampPattern currentStep = steps[patternSteps.CurrentStepIndex];

                        if (currentStep.IsValid)
                        {
                            yield return ExecuteStepLight(currentStep);
                        }
                        else
                        {
                            yield return ExecuteDefaultLight();
                        }
                    }
                }
                yield return WaitForSeconds(delayAfterComplete, useUnscaledTime);
                yield return isInverse ? ExecuteInverseDisabling() : ExecuteNormalDisabling();
                yield return WaitForSeconds(restartDelay, useUnscaledTime);

                if (zigzag)
                {
                    isInverse = !isInverse;
                }

                lampsSteps.SetStep(isInverse ? lamps.Length - 1 : 0);
                patternSteps.NextStep();
                usedLamps = 0;
                currentCycle++;
                resetThisFrame = false;
            }

            bool CheckWhileCondition()
            {
                return !resetThisFrame;
            }

            void SetNextLight(bool isActive)
            {
                usedLamps++;
                ILamp lamp = lamps[lampsSteps.CurrentStepIndex];
                lamp.SetActiveState(isActive);

                if (isInverse)
                {
                    if (lampsSteps.CurrentStepIndex == 0)
                    {
                        resetThisFrame = true;
                    }
                    lampsSteps.PreviousStep();
                }
                else
                {
                    if (lampsSteps.CurrentStepIndex == lamps.Length - 1)
                    {
                        resetThisFrame = true;
                    }
                    lampsSteps.NextStep();
                }

                if (usedLamps > maxOnLamps)
                {
                    DisableOldestLamp();
                }
            }

            void DisableOldestLamp()
            {
                int lampIndex = isInverse
                    ? (lampsSteps.CurrentStepIndex + maxOnLamps) % lamps.Length
                    : (lampsSteps.CurrentStepIndex - maxOnLamps + lamps.Length) % lamps.Length;

                DisableLamp(lamps[lampIndex]);
            }

            void DisableLamp(ILamp lamp)
            {
                lamp.Deactivate();
                usedLamps--;
            }

            IEnumerator ExecuteStepLight(JumpLampPattern steps)
            {
                for (int i = 0; i < steps.Lightcount; i++)
                {
                    SetNextLight(steps.IsLightOn(i + (currentCycle * cycleOffset)));
                    yield return WaitForSeconds(turnOnDelay, useUnscaledTime);
                }
            }

            IEnumerator ExecuteDefaultLight()
            {
                while (CheckWhileCondition())
                {
                    SetNextLight(true);
                    yield return WaitForSeconds(turnOnDelay, useUnscaledTime);
                }
            }

            IEnumerator ExecuteInverseDisabling()
            {
                for (int i = maxOnLamps - 1; i >= 0; i--)
                {
                    if (lampsSteps.CurrentStepIndex + i < lamps.Length)
                    {
                        DisableLamp(lamps[lampsSteps.CurrentStepIndex + i]);
                    }
                    yield return WaitForSeconds(turnOnDelay, useUnscaledTime);
                }
            }

            IEnumerator ExecuteNormalDisabling()
            {
                for (int i = maxOnLamps - 1; i >= 0; i--)
                {
                    if (lampsSteps.CurrentStepIndex - i >= 0)
                    {
                        DisableLamp(lamps[lampsSteps.CurrentStepIndex - i]);
                    }
                    yield return WaitForSeconds(turnOnDelay, useUnscaledTime);
                }
            }
        }
    }
}