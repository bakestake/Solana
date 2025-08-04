using UnityEngine;

namespace Gamegaard.BasicBehaviours
{
    [CreateAssetMenu(fileName = "CosineCycle", menuName = "CyclicBehaviours/Cosine")]
    public class CosineBehaviourData : CyclicBehaviourData
    {
        private const float sinHalfCycle = Mathf.PI;

        public override float Evaluate(float time, float frequency, float amplitude)
        {
            return Mathf.Cos(time * sinHalfCycle * frequency) * amplitude;
        }
    }
}