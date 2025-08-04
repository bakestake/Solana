using UnityEngine;

namespace Gamegaard
{
    public abstract class TwoStateObject : MonoBehaviour, IStateObject
    {
        [field: SerializeField] public bool IsActive { get; private set; }

        protected virtual bool CanChangeState()
        {
            return true;
        }

        public void Activate()
        {
            if (!CanChangeState()) return;
            OnBecameActive();
        }

        public void Deactivate()
        {
            if (!CanChangeState()) return;
            OnBecameInactive();
        }

        public void Toggle()
        {
            if (IsActive)
            {
                Deactivate();
            }
            else
            {
                Activate();
            }
        }

        protected virtual void OnBecameActive()
        {
            IsActive = true;
        }

        protected virtual void OnBecameInactive()
        {
            IsActive = false;
        }
    }
}