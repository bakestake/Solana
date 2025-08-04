using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game.UI.InLevelUpgrades
{
    public abstract class HoldOnObjectBase : MonoBehaviour, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private UnityEvent OnCompleted;

        public bool IsHolding { get; protected set; }
        public bool IsCompleted { get; protected set; }

        protected virtual void Update()
        {
            if (CheckCompletion())
            {
                IsCompleted = true;
                OnComplete();
            }
        }

        protected virtual void OnComplete()
        {
            OnCompleted?.Invoke();
        }

        public virtual void ResetValues()
        {
            IsCompleted = false;
            IsHolding = false;
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            IsHolding = true;
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            CancelHoldOn();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            CancelHoldOn();
        }

        private void CancelHoldOn()
        {
            IsHolding = false;
            if (!IsCompleted)
            {
                ResetValues();
            }
        }

        protected abstract bool CheckCompletion();
    }
}