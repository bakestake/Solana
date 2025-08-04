using UnityEngine;

namespace Gamegaard.BasicBehaviours
{
    [CreateAssetMenu(fileName = "PingPongCycle", menuName = "CyclicBehaviours/PingPong")]
    public class PingPongBehaviourData : CyclicBehaviourData
    {
        public override float Evaluate(float time, float frequency, float amplitude)
        {
            return Mathf.PingPong(time * frequency, 1) * amplitude;
        }
    }
}
