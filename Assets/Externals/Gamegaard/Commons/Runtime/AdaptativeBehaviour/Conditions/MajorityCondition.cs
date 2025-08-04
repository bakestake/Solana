using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard.AdaptativeBehavior
{
    public class MajorityCondition : IConditionEvaluator
    {
        public bool Evaluate(IEnumerable<IAdaptativeBehaviorCondition> conditions, GameObject target)
        {
            int total = 0;
            int trueCount = 0;

            foreach (IAdaptativeBehaviorCondition condition in conditions)
            {
                total++;
                if (condition.Evaluate(target))
                    trueCount++;
            }

            return trueCount > total / 2;
        }
    }
}