using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard.AdaptativeBehavior
{
    [System.Serializable]
    public class AtMostNCondition : IConditionEvaluator
    {
        [SerializeField] private int maxCount;

        public AtMostNCondition()
        {
        }

        public AtMostNCondition(int maxCount)
        {
            this.maxCount = maxCount;
        }

        public bool Evaluate(IEnumerable<IAdaptativeBehaviorCondition> conditions, GameObject target)
        {
            int trueCount = 0;

            foreach (IAdaptativeBehaviorCondition condition in conditions)
            {
                if (condition.Evaluate(target))
                    trueCount++;

                if (trueCount > maxCount)
                    return false;
            }

            return true;
        }
    }
}