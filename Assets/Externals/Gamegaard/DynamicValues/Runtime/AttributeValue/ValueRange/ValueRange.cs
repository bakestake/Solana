using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Represents an abstract range of values, defining maximum, minimum, and current values with additional utility methods.
    /// </summary>
    /// <typeparam name="TValue">The type of the values in the range, constrained to numeric-compatible types.</typeparam>
    [Serializable]
    public abstract class ValueRange<TValue> : INumberRange<TValue> where TValue : struct, IComparable, IComparable<TValue>, IConvertible, IEquatable<TValue>, IFormattable
    {
        [SerializeField] protected TValue _maxValue;
        [SerializeField] protected TValue _minValue;
        [SerializeField] protected TValue _currentValue;

        public ValueRange()
        {
        }

        public ValueRange(TValue maxValue, TValue currentValue, TValue minValue = default)
        {
            _maxValue = maxValue;
            _minValue = minValue;
            _currentValue = currentValue;
        }

        public ValueRange(TValue maxValue, TValue minValue = default) : this(maxValue, maxValue, minValue)
        {

        }

        public ValueRange(ValueRange<TValue> other) : this(other._maxValue, other._currentValue, other._minValue)
        {

        }

        public abstract float Percentage { get; }
        public abstract float InversePercentage { get; }
        public abstract bool IsFull { get; }
        public abstract bool HasValue { get; }
        public abstract bool IsZero { get; }
        public abstract TValue MissingValue { get; }

        public virtual TValue MaxValue
        {
            get => _maxValue;
            set => _maxValue = value;
        }

        public virtual TValue MinValue
        {
            get => _minValue;
            set => _minValue = value;
        }

        public abstract TValue CurrentValue { get; set; }


        public event Action<ValueRange<TValue>> OnValueChanged;

        /// <summary>
        /// Calls the value changed event.
        /// </summary>
        protected void CallEvents()
        {
            OnValueChanged?.Invoke(this);
        }

        public void SetToMaxValue()
        {
            CurrentValue = _maxValue;
        }

        public void SetToMinValue()
        {
            CurrentValue = _minValue;
        }

        public abstract void SetToPercentage(float percentage);

        public virtual string ToFormattedText() => $"{_currentValue}/{_maxValue}";

        /// <summary>
        /// Compares this range with another range.
        /// </summary>
        /// <param name="other">The other range to compare with.</param>
        /// <returns>
        /// -1 if this range is less than the other range, 0 if equal, 1 if greater.
        /// </returns>
        public int CompareTo(ValueRange<TValue> other)
        {
            return Comparer<TValue>.Default.Compare(_currentValue, other._currentValue);
        }

        /// <summary>
        /// Checks if the current value is greater than another range's current value.
        /// </summary>
        public bool IsGreaterThan(ValueRange<TValue> other)
        {
            return CompareTo(other) > 0;
        }

        /// <summary>
        /// Checks if the current value is less than another range's current value.
        /// </summary>
        public bool IsLessThan(ValueRange<TValue> other)
        {
            return CompareTo(other) < 0;
        }

        public override string ToString()
        {
            return $"{CurrentValue}/{MaxValue}";
        }
    }
}