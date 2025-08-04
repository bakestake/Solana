using System;
using UnityEngine;

namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Extends <see cref="DynamicInt"/> to include range-specific functionality, supporting dynamic integer values with additional range operations.
    /// </summary>
    [Serializable]
    public class DynamicIntRange : DynamicInt, INumberRange<int>
    {
        [SerializeField] private IntRange _range;

        public int CurrentValue
        {
            get => _range.CurrentValue;
            set
            {
                RecalculateIfDirty();
                _range.CurrentValue = value;
                CallEvents();
            }
        }

        public int MaxValue => _range.MaxValue;
        public int MinValue => _range.MinValue;
        public float Percentage => _range.Percentage;
        public float InversePercentage => _range.InversePercentage;
        public bool IsFull => _range.IsFull;
        public bool HasValue => _range.HasValue;
        public bool IsZero => _range.IsZero;
        public int MissingValue => _range.MissingValue;

        public event Action<DynamicIntRange> OnCurrentValueChanged;

        public DynamicIntRange() : base() { }

        public DynamicIntRange(int baseValue) : base(baseValue)
        {
            _range = new IntRange(baseValue, baseValue, 0);
        }

        public DynamicIntRange(int baseValue, int currentValue) : base(baseValue)
        {
            _range = new IntRange(baseValue, currentValue, 0);
        }

        public DynamicIntRange(DynamicIntRange other) : this(other.BaseValue, other.CurrentValue)
        {
        }

        protected override void OnCalculatedValueChanged()
        {
            base.OnCalculatedValueChanged();
            _range.MaxValue = _calculatedValue;
        }

        public override void CallEvents()
        {
            base.CallEvents();
            OnCurrentValueChanged?.Invoke(this);
        }

        public void SetToMaxValue()
        {
            _range.SetToMaxValue();
            CallEvents();
        }

        public void SetToMinValue()
        {
            _range.SetToMinValue();
            CallEvents();
        }

        public void SetToPercentage(float percentage)
        {
            _range.SetToPercentage(percentage);
            CallEvents();
        }

        public override string ToString()
        {
            return _range.ToFormattedText();
        }

        public override void ChangeBaseValue(int newValue)
        {
            base.ChangeBaseValue(newValue);
            if(newValue < CurrentValue)
            {
                CurrentValue = newValue;
            }
        }

        #region Operators
        public static explicit operator int(DynamicIntRange value) => value.CurrentValue;
        public static explicit operator long(DynamicIntRange value) => value.CurrentValue;
        public static explicit operator float(DynamicIntRange value) => value.CurrentValue;
        public static explicit operator double(DynamicIntRange value) => value.CurrentValue;
        public static explicit operator decimal(DynamicIntRange value) => value.CurrentValue;

        public static DynamicIntRange operator ++(DynamicIntRange a)
        {
            a.CurrentValue++;
            return a;
        }

        public static DynamicIntRange operator --(DynamicIntRange a)
        {
            a.CurrentValue--;
            return a;
        }

        public static DynamicIntRange operator +(DynamicIntRange a, int b)
        {
            a.CurrentValue += b;
            return a;
        }

        public static int operator +(int a, DynamicIntRange b)
        {
            a += b.CurrentValue;
            return a;
        }

        public static DynamicIntRange operator -(DynamicIntRange a, int b)
        {
            a.CurrentValue -= b;
            return a;
        }

        public static int operator -(int a, DynamicIntRange b)
        {
            a -= b.CurrentValue;
            return a;
        }

        public static DynamicIntRange operator *(DynamicIntRange a, int scalar)
        {
            a.CurrentValue *= scalar;
            return a;
        }

        public static int operator *(int a, DynamicIntRange scalar)
        {
            a *= scalar.CurrentValue;
            return a;
        }

        public static int operator /(int a, DynamicIntRange scalar)
        {
            a /= scalar.CurrentValue;
            return a;
        }

        public static DynamicIntRange operator /(DynamicIntRange a, int scalar)
        {
            a.CurrentValue /= scalar;
            return a;
        }
        #endregion
    }
}