using UnityEngine;

namespace Gamegaard.BasicBehaviours
{
    public abstract class CyclicBehaviourData : ScriptableObject
    {
        public abstract float Evaluate(float time, float frequency, float amplitude);
    }
}