using UnityEngine;

namespace Gamegaard.BasicBehaviours
{
    public abstract class ValuedCyclicBehaviour<T, G> : CyclicBehaviour
    {
        [SerializeField] protected T direction;
        protected G initialValue;
        protected abstract G CurrentValue { get; set; }

        protected override void Awake()
        {
            base.Awake();
            initialValue = CurrentValue;
        }

        protected override void Update()
        {
            base.Update();
            CurrentValue = GetEvaluatedValue();
        }

        protected abstract G GetEvaluatedValue();
    }
}