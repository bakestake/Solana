using UnityEngine;
using UnityEngine.Events;

namespace Gamegaard
{
    public class TwoStateEventCaster : TwoStateObject
    {
        [SerializeField] private UnityEvent OnSetOn;
        [SerializeField] private UnityEvent OnSetOff;

        protected override void OnBecameActive()
        {
            base.OnBecameActive();
            OnSetOn?.Invoke();
        }

        protected override void OnBecameInactive()
        {
            base.OnBecameInactive();
            OnSetOff?.Invoke();
        }
    }
}