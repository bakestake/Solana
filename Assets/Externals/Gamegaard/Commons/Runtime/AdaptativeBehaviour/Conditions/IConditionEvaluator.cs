using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard.AdaptativeBehavior
{
    public interface IConditionEvaluator
    {
        bool Evaluate(IEnumerable<IAdaptativeBehaviorCondition> conditions, GameObject target);
    }
}