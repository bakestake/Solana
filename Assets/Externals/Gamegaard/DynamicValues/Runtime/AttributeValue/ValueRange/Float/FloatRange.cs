using UnityEngine;

namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Represents a floating-point range with utility methods for managing and calculating values within the range.
    /// </summary>
    [System.Serializable]
    public class FloatRange : ValueRange<float>
    {
        public override float Percentage => _currentValue / _maxValue;
        public override float InversePercentage => 1 - Percentage;
        public override bool IsFull => _currentValue >= _maxValue;
        public override bool HasValue => _currentValue > 0;
        public override bool IsZero => _currentValue == 0;
        public override float MissingValue => _maxValue - _currentValue;

        public override float MaxValue
        {
            set
            {
                _maxValue = Mathf.Max(value, _minValue);
                _currentValue = Mathf.Clamp(_currentValue, _minValue, _maxValue);
                base.MaxValue = _maxValue;
            }
        }

        public override float MinValue
        {
            set
            {
                _minValue = Mathf.Min(value, _maxValue);
                _currentValue = Mathf.Clamp(_currentValue, _minValue, _maxValue);
                base.MinValue = _minValue;
            }
        }

        public override float CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = Mathf.Clamp(value, _minValue, _maxValue);
                CallEvents();
            }
        }

        public FloatRange() : base()
        {
            _maxValue = float.MaxValue;
        }

        public FloatRange(ValueRange<float> other) : base(other) { }
        public FloatRange(float maxValue, float currentValue, float minValue = 0) : base(maxValue, currentValue, minValue) { }
        public FloatRange(float maxValue, float minValue = 0) : this(maxValue, maxValue, minValue) { }

        public override void SetToPercentage(float percentage)
        {
            CurrentValue = _minValue + ((_maxValue - _minValue) * percentage);
        }
    }
}