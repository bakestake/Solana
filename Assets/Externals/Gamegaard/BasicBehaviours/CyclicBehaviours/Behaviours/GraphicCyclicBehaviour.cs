using UnityEngine;

namespace Gamegaard.BasicBehaviours
{
    public abstract class GraphicCyclicBehaviour : ValuedCyclicBehaviour<Gradient, Color>
    {
        protected override Color GetEvaluatedValue()
        {
            return direction.Evaluate(EvaluatedValue);
        }
    }
}