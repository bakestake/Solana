using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard.AdaptativeBehavior
{
    public class XorCondition : IConditionEvaluator
    {
        public bool Evaluate(IEnumerable<IAdaptativeBehaviorCondition> conditions, GameObject target)
        {
            bool foundOneTrue = false;

            foreach (IAdaptativeBehaviorCondition condition in conditions)
            {
                if (condition.Evaluate(target))
                {
                    if (foundOneTrue)
                        return false;
                    foundOneTrue = true;
                }
            }

            return foundOneTrue;
        }
    }
}