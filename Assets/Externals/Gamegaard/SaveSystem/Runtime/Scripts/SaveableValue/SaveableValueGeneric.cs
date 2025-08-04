using System;

namespace Everseeker
{
    public abstract class SaveableValueGeneric<T> : SaveableValue
    {
        protected T value;

        /// <summary>
        /// Dont save value on changed.
        /// </summary>
        public virtual T Value
        {
            get => value; set
            {
                this.value = value;
                OnValueChanged?.Invoke(value);
            }
        }

        public event Action<T> OnValueChanged;
        /// <summary>
        /// Save value on changed.
        /// </summary>
        public virtual T AutoSaveableValue
        {
            get => value;
            set
            {
                this.value = value;
                Save();
                OnValueChanged?.Invoke(value);
            }
        }

        public SaveableValueGeneric(string saveKey, T defaultValue = default) : base(saveKey)
        {
            value = defaultValue;
        }

        public abstract void Load(T defaultValue = default);
    }
}