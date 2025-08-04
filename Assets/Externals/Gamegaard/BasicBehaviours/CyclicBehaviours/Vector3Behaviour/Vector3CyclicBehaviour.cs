using UnityEngine;

namespace Gamegaard.BasicBehaviours
{
    public abstract class Vector3CyclicBehaviour : ValuedCyclicBehaviour<Vector3, Vector3>
    {
        protected override Vector3 GetEvaluatedValue()
        {
            return initialValue + (direction * EvaluatedValue);
        }
    }
}