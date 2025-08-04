using UnityEngine;

namespace Gamegaard.BasicBehaviours
{
    [CreateAssetMenu(fileName = "AnimCurveCycle_", menuName = "CyclicBehaviours/AnimationCurve")]
    public class AnimCurveBehaviourData : CyclicBehaviourData
    {
        [SerializeField] private AnimationCurve curve;
        public override float Evaluate(float time, float frequency, float amplitude)
        {
            float t = Mathf.PingPong(time * frequency, 1) * amplitude;
            return curve.Evaluate(t);
        }
    }
}
