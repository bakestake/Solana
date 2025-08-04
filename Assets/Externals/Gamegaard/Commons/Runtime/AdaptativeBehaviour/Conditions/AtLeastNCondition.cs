using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard.AdaptativeBehavior
{
    [System.Serializable]
    public class AtLeastNCondition : IConditionEvaluator
    {
        [SerializeField] private int minCount;

        public AtLeastNCondition()
        {
        }

        public AtLeastNCondition(int minCount)
        {
            this.minCount = minCount;
        }

        public bool Evaluate(IEnumerable<IAdaptativeBehaviorCondition> conditions, GameObject target)
        {
            int trueCount = 0;

            foreach (IAdaptativeBehaviorCondition condition in conditions)
            {
                if (condition.Evaluate(target))
                    trueCount++;

                if (trueCount >= minCount)
                    return true; 
            }

            return false;
        }
    }
}