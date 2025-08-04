using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard.AdaptativeBehavior
{
    public class OrCondition : IConditionEvaluator
    {
        public bool Evaluate(IEnumerable<IAdaptativeBehaviorCondition> conditions, GameObject target)
        {
            foreach (IAdaptativeBehaviorCondition condition in conditions)
            {
                if (condition.Evaluate(target))
                    return true;
            }
            return false;
        }
    }
}