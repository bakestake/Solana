using UnityEngine;

namespace Gamegaard.BasicBehaviours
{
    [CreateAssetMenu(fileName = "SineCycle", menuName = "CyclicBehaviours/Sine")]
    public class SineBehaviourData : CyclicBehaviourData
    {
        private const float sinHalfCycle = Mathf.PI;

        public override float Evaluate(float time, float frequency, float amplitude)
        {
            return Mathf.Sin(time * sinHalfCycle * frequency) * amplitude;
        }
    }
}