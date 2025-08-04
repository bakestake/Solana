using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard.AdaptativeBehavior
{
    public class NotCondition : IConditionEvaluator
    {
        public bool Evaluate(IEnumerable<IAdaptativeBehaviorCondition> conditions, GameObject target)
        {
            foreach (IAdaptativeBehaviorCondition condition in conditions)
            {
                if (condition.Evaluate(target))
                    return false;
            }
            return true;
        }
    }
}