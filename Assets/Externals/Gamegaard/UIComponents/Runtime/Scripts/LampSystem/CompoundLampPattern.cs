using System.Collections;
using UnityEngine;

namespace Gamegaard
{
    [CreateAssetMenu(fileName = "CompoundLampPattern_", menuName = "LampPatterns/CompoundLampPattern")]
    public class CompoundLampPattern : LampPattern
    {
        [SerializeField] private CompoundLampCyclePattern[] patterns;

        public override IEnumerator ExecutePattern(ILamp[] lamps, int cyclesCount = -1, bool useUnscaledTime = false)
        {
            int currentCycle = 0;

            while (cyclesCount == -1 || currentCycle < cyclesCount)
            {
                if (patterns.Length == 0)
                {
                    yield return ExecuteDefaultLight();
                }
                else
                {
                    foreach (CompoundLampCyclePattern pattern in patterns)
                    {
                        yield return pattern.pattern.ExecutePattern(lamps, pattern.cyclesCount, useUnscaledTime);

                    }
                }

                currentCycle++;
            }
        }

        private IEnumerator ExecuteDefaultLight()
        {
            yield return null;
        }
    }
}