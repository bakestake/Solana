using System;

namespace Everseeker
{
    public class EventCasterValue<T>
    {
        protected T value;

        public event Action<T> OnValueChanged;

        public T Value
        {
            get => value;
            set
            {
                this.value = value;
                OnValueChanged?.Invoke(value);
            }
        }

        public EventCasterValue(T value)
        {
            this.value = value;
        }

        public void TriggerEvent()
        {
            OnValueChanged?.Invoke(value);
        }
    }
}