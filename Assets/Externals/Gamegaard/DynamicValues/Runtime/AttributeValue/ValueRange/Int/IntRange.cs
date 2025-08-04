using UnityEngine;

namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Represents an integer range with utility methods for managing and calculating values within the range.
    /// </summary>
    [System.Serializable]
    public class IntRange : ValueRange<int>
    {
        public override float Percentage => (float)_currentValue / _maxValue;
        public override float InversePercentage => 1 - Percentage;
        public override bool IsFull => _currentValue >= _maxValue;
        public override bool HasValue => _currentValue > 0;
        public override bool IsZero => _currentValue == 0;
        public override int MissingValue => _maxValue - _currentValue;

        public override int MaxValue
        {
            set
            {
                _maxValue = Mathf.Max(value, _minValue);
                _currentValue = Mathf.Clamp(_currentValue, _minValue, _maxValue);
                base.MaxValue = _maxValue;
            }
        }

        public override int MinValue
        {
            set
            {
                _minValue = Mathf.Min(value, _maxValue);
                _currentValue = Mathf.Clamp(_currentValue, _minValue, _maxValue);
                base.MinValue = _minValue;
            }
        }

        public override int CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = Mathf.Clamp(value, _minValue, _maxValue);
                CallEvents();
            }
        }

        public IntRange() : base()
        {
            _minValue = int.MinValue;
            _maxValue = int.MaxValue;
        }

        public IntRange(ValueRange<int> other) : base(other) { }
        public IntRange(int maxValue, int currentValue, int minValue = 0) : base(maxValue, currentValue, minValue) { }
        public IntRange(int maxValue, int minValue = 0) : this(maxValue, maxValue, minValue) { }

        public override void SetToPercentage(float percentage)
        {
            CurrentValue = _minValue + (int)((_maxValue - _minValue) * percentage);
        }
    }
}