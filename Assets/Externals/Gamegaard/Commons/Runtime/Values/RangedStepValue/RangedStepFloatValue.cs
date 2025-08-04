using System;
using UnityEngine;

namespace Gamegaard.CustomValues
{
    [Serializable]
    public class RangedStepFloatValue
    {
        [SerializeField] private MinMaxFloat range = new MinMaxFloat(0.5f, 2);
        [Min(0)]
        [SerializeField] private float step = 0.1f;

        private float initialValue;
        private float currentValue;

        public float MinValue => range.Min;
        public float MaxValue => range.Max;

        public event Action<float> OnValueChanged;

        public RangedStepFloatValue()
        {
            initialValue = 1;
            currentValue = 1;
        }

        public RangedStepFloatValue(float initialValue)
        {
            currentValue = initialValue;
            this.initialValue = initialValue;
        }

        public RangedStepFloatValue(float initialValue, MinMaxFloat range, float step = 0.1f)
        {
            currentValue = initialValue;
            this.initialValue = initialValue;
            this.range = range;
            this.step = step;
        }

        public float CurrentValue
        {
            get => currentValue;
            set
            {
                currentValue = range.Clamp(Mathf.Round(value * 10f) / 10f);
                TriggerEvent();
            }
        }

        public void SetInitialValue(float value)
        {
            initialValue = value;
        }

        public void TriggerEvent()
        {
            OnValueChanged?.Invoke(CurrentValue);
        }

        public virtual void AddStepValue()
        {
            CurrentValue += step;
        }

        public virtual void SubtractStepValue()
        {
            CurrentValue -= step;
        }

        public virtual void Reset()
        {
            CurrentValue = initialValue;
        }
    }
}