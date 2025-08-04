using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gamegaard.AdaptativeBehavior
{
    public class AdaptativeBehavior : MonoBehaviour
    {
        [SerializeField] private List<ConditionCheck> conditions;
        [SerializeReference] private List<IAdaptativeBehaviorEffect> effects;
        [SerializeField] private UnityEvent OnConditionsMeet;

        private void Start()
        {
            CheckAndApply();
        }

        public void CheckAndApply()
        {
            if (EvaluateConditions())
            {
                foreach (IAdaptativeBehaviorEffect effect in effects)
                {
                    effect.Apply(gameObject);
                }

                OnConditionsMeet?.Invoke();
            }
        }

        private bool EvaluateConditions()
        {
            if (conditions == null || conditions.Count == 0) return false;

            foreach (ConditionCheck conditionCheck in conditions)
            {
                if (!conditionCheck.Evaluate(gameObject)) return false;
            }

            return true;
        }
    }
}