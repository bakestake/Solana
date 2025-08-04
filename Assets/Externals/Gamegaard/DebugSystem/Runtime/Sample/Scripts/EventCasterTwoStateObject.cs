using UnityEngine;
using UnityEngine.Events;

namespace Gamegaard.RuntimeDebug
{
    public class EventCasterTwoStateObject : TwoStateObject
    {
        [SerializeField] private UnityEvent OnEnable;
        [SerializeField] private UnityEvent OnDisable;

        public override void Active()
        {
            base.Active();
            OnEnable?.Invoke();
        }

        public override void Desactive()
        {
            base.Desactive();
            OnDisable?.Invoke();
        }
    }
}