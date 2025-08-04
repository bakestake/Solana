using System;
using UnityEngine;

namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Extends <see cref="DynamicFloat"/> to include range-specific functionality, supporting dynamic floating-point values with additional range operations.
    /// </summary>
    [Serializable]
    public class DynamicFloatRange : DynamicFloat, INumberRange<float>
    {
        [SerializeField] private FloatRange _range;

        public float CurrentValue
        {
            get => _range.CurrentValue;
            set
            {
                RecalculateIfDirty();
                _range.CurrentValue = value;
                CallEvents();
            }
        }

        public float MaxValue => _range.MaxValue;
        public float MinValue => _range.MinValue;
        public float Percentage => _range.Percentage;
        public float InversePercentage => _range.InversePercentage;
        public bool IsFull => _range.IsFull;
        public bool HasValue => _range.HasValue;
        public bool IsZero => _range.IsZero;
        public float MissingValue => _range.MissingValue;

        public event Action<DynamicFloatRange> OnCurrentValueChanged;

        public DynamicFloatRange() : base()
        {
            _range = new FloatRange();
        } 

        public DynamicFloatRange(float baseValue) : base(baseValue)
        {
            _range = new FloatRange(baseValue, baseValue, 0);
        }

        public DynamicFloatRange(float baseValue, float currentValue) : base(baseValue)
        {
            _range = new FloatRange(baseValue, currentValue, 0);
        }

        public DynamicFloatRange(DynamicFloatRange other) : this(other.BaseValue, other.CurrentValue)
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

        public override void ChangeBaseValue(float newValue)
        {
            base.ChangeBaseValue(newValue);
            if (newValue < CurrentValue)
            {
                CurrentValue = newValue;
            }
        }

        #region Operators
        public static explicit operator int(DynamicFloatRange value) => (int)value.CurrentValue;
        public static explicit operator long(DynamicFloatRange value) => (long)value.CurrentValue;
        public static explicit operator float(DynamicFloatRange value) => value.CurrentValue;
        public static explicit operator double(DynamicFloatRange value) => value.CurrentValue;
        public static explicit operator decimal(DynamicFloatRange value) => (decimal)value.CurrentValue;

        public static DynamicFloatRange operator ++(DynamicFloatRange a)
        {
            a.CurrentValue++;
            return a;
        }

        public static DynamicFloatRange operator --(DynamicFloatRange a)
        {
            a.CurrentValue--;
            return a;
        }

        public static DynamicFloatRange operator +(DynamicFloatRange a, float b)
        {
            a.CurrentValue += b;
            return a;
        }

        public static float operator +(float a, DynamicFloatRange b)
        {
            a += b.CurrentValue;
            return a;
        }

        public static DynamicFloatRange operator -(DynamicFloatRange a, float b)
        {
            a.CurrentValue -= b;
            return a;
        }

        public static float operator -(float a, DynamicFloatRange b)
        {
            a -= b.CurrentValue;
            return a;
        }

        public static DynamicFloatRange operator *(DynamicFloatRange a, float scalar)
        {
            a.CurrentValue *= scalar;
            return a;
        }

        public static float operator *(float a, DynamicFloatRange scalar)
        {
            a *= scalar.CurrentValue;
            return a;
        }

        public static float operator /(float a, DynamicFloatRange scalar)
        {
            a /= scalar.CurrentValue;
            return a;
        }

        public static DynamicFloatRange operator /(DynamicFloatRange a, float scalar)
        {
            a.CurrentValue /= scalar;
            return a;
        }

        public static DynamicFloatRange operator +(DynamicFloatRange a, int b)
        {
            a.CurrentValue += b;
            return a;
        }

        public static int operator +(int a, DynamicFloatRange b)
        {
            a += (int)b.CurrentValue;
            return a;
        }

        public static DynamicFloatRange operator -(DynamicFloatRange a, int b)
        {
            a.CurrentValue -= b;
            return a;
        }

        public static int operator -(int a, DynamicFloatRange b)
        {
            a -= (int)b.CurrentValue;
            return a;
        }

        public static DynamicFloatRange operator *(DynamicFloatRange a, int scalar)
        {
            a.CurrentValue *= scalar;
            return a;
        }

        public static int operator *(int a, DynamicFloatRange scalar)
        {
            a = (int)(a * scalar.CurrentValue);
            return a;
        }

        public static int operator /(int a, DynamicFloatRange scalar)
        {
            a = (int)(a / scalar.CurrentValue);
            return a;
        }

        public static DynamicFloatRange operator /(DynamicFloatRange a, int scalar)
        {
            a.CurrentValue /= scalar;
            return a;
        }
        #endregion
    }
}