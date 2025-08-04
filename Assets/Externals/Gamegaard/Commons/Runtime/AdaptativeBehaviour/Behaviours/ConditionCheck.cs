using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard.AdaptativeBehavior
{
    [System.Serializable]
    public class ConditionCheck : IAdaptativeBehaviorCondition
    {
        [TypeSearch(typeof(IConditionEvaluator))]
        [SerializeReference] private IConditionEvaluator conditionEvaluator;

        [SerializeReference] private List<IAdaptativeBehaviorCondition> conditions;

        public bool Evaluate(GameObject target)
        {
            return conditionEvaluator != null && conditionEvaluator.Evaluate(conditions, target);
        }
    }
}