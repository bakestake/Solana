using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard.AdaptativeBehavior
{
    [System.Serializable]
    public class ExactlyNCondition : IConditionEvaluator
    {
        [SerializeField] private int requiredCount;

        public ExactlyNCondition()
        {
        }

        public ExactlyNCondition(int requiredCount)
        {
            this.requiredCount = requiredCount;
        }

        public bool Evaluate(IEnumerable<IAdaptativeBehaviorCondition> conditions, GameObject target)
        {
            int trueCount = 0;

            foreach (IAdaptativeBehaviorCondition condition in conditions)
            {
                if (condition.Evaluate(target))
                    trueCount++;
            }

            return trueCount == requiredCount;
        }
    }
}