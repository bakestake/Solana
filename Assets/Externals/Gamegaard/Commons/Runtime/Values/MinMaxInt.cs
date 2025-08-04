using System;
using System.Globalization;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gamegaard.CustomValues
{
    [Serializable]
    public struct MinMaxInt : IEquatable<MinMaxInt>, IEquatable<Vector2Int>, IFormattable
    {
        [SerializeField] private int min;
        [SerializeField] private int max;

        private static readonly MinMaxInt zero = new MinMaxInt(0, 0);
        private static readonly MinMaxInt one = new MinMaxInt(1, 1);
        private static readonly MinMaxInt zeroAndPositive = new MinMaxInt(0, 1);
        private static readonly MinMaxInt zeroAndNegative = new MinMaxInt(0, -1);
        private static readonly MinMaxInt negativeAndZero = new MinMaxInt(-1, 0);
        private static readonly MinMaxInt positiveAndZero = new MinMaxInt(1, 0);
        private const int DefaultMaxTries = 100;

        /// <summary>
        /// Gets or sets the minimum value of the range.
        /// </summary>
        /// <value>The minimum value of the range.</value>
        public int Min { get => min; private set => min = value; }

        /// <summary>
        /// Gets or sets the maximum value of the range.
        /// </summary>
        /// <value>The maximum value of the range.</value>
        public int Max { get => max; private set => max = value; }

        /// <summary>
        /// Initializes a new instance of the MinMaxInt struct with the specified minimum and maximum values.
        /// </summary>
        /// <param name="min">The minimum value of the range.</param>
        /// <param name="max">The maximum value of the range.</param>
        public MinMaxInt(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public static MinMaxInt Zero => zero;
        public static MinMaxInt One => one;
        public static MinMaxInt ZeroAndPositive => zeroAndPositive;
        public static MinMaxInt ZeroAndNegative => zeroAndNegative;
        public static MinMaxInt NegativeAndZero => negativeAndZero;
        public static MinMaxInt PositiveAndZero => positiveAndZero;

        /// <summary>
        /// Returns a random integer value between the minimum and maximum values.
        /// </summary>
        /// <returns>A random integer value within the range.</returns>
        public int GetRandom()
        {
            return Random.Range(min, max);
        }

        /// <summary>
        /// Returns a random integer value within the specified range, ensuring that the random value differs from baseValue by at least minVariation.
        /// If the condition is not met after maxTries attempts, the last generated value is returned.
        /// </summary>
        /// <param name="baseValue">The base value to compare the variation.</param>
        /// <param name="minVariation">The minimum required variation from the base value.</param>
        /// <param name="maxTries">The maximum number of attempts to find a value with sufficient variation. Default is 100.</param>
        /// <returns>A random integer value that meets the variation requirement.</returns>
        public int GetRandomValueInRange(int baseValue, int minVariation, int maxTries = DefaultMaxTries)
        {
            int minValue = baseValue + min;
            int maxValue = baseValue + max;

            for (int i = 0; i < maxTries; i++)
            {
                int random = Random.Range(minValue, maxValue);

                if (Mathf.Abs(random - baseValue) >= minVariation)
                {
                    Debug.Log($"{baseValue}, {minVariation} = {random} ({Mathf.Abs(random - baseValue)})");
                    return random;
                }
            }

            Debug.LogWarning($"Max tries reached. Minimum variation ({minVariation}) might not be satisfied. Returning baseValue: {baseValue}");
            return baseValue;
        }

        /// <summary>
        /// Sets the minimum value of the range.
        /// </summary>
        /// <param name="min">The new minimum value.</param>
        public void SetMin(int min)
        {
            this.min = min;
        }

        /// <summary>
        /// Sets the maximum value of the range.
        /// </summary>
        /// <param name="max">The new maximum value.</param>
        public void SetMax(int max)
        {
            this.max = max;
        }

        /// <summary>
        /// Sets both the minimum and maximum values of the range.
        /// </summary>
        /// <param name="min">The new minimum value.</param>
        /// <param name="max">The new maximum value.</param>
        public void SetValue(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Sets the range values to those of another MinMaxInt instance.
        /// </summary>
        /// <param name="other">The other MinMaxInt instance to copy values from.</param>
        public void SetValue(MinMaxInt other)
        {
            min = other.min;
            max = other.max;
        }

        /// <summary>
        /// Sets the minimum and maximum values of the range using the components of a Vector2Int.
        /// </summary>
        /// <param name="vector">The Vector2Int instance where x is used as the minimum and y as the maximum value.</param>
        public void SetValue(Vector2Int vector)
        {
            min = vector.x;
            max = vector.y;
        }

        /// <summary>
        /// Restricts a value to be within the range defined by the minimum and maximum values.
        /// </summary>
        /// <param name="value">The value to be clamped.</param>
        /// <returns>The clamped integer result between the minimum and maximum values.</returns>
        public int Clamp(int value)
        {
            return Mathf.Clamp(value, min, max);
        }

        /// <summary>
        /// Checks whether this instance is equal to another object.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>True if the objects are equal, otherwise false.</returns>
        public override bool Equals(object other)
        {
            if (other is MinMaxInt minMax)
            {
                return Equals(minMax);
            }

            return false;
        }

        /// <summary>
        /// Checks whether this instance is equal to another MinMaxInt.
        /// </summary>
        /// <param name="other">The MinMaxInt instance to compare.</param>
        /// <returns>True if both instances are equal, otherwise false.</returns>
        public bool Equals(MinMaxInt other)
        {
            return min == other.min && max == other.max;
        }

        /// <summary>
        /// Checks whether this instance is equal to a Vector2Int, treating x as min and y as max.
        /// </summary>
        /// <param name="other">The Vector2Int instance to compare.</param>
        /// <returns>True if the values are equal, otherwise false.</returns>
        public bool Equals(Vector2Int other)
        {
            return min == other.x && max == other.y;
        }

        /// <summary>
        /// Returns the hash code for this instance, based on the min and max values.
        /// </summary>
        /// <returns>The hash code as an integer.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(min, max);
        }

        /// <summary>
        /// Converts the MinMaxInt instance to a string representation with a specified format and culture information.
        /// </summary>
        /// <param name="format">The format for the integer values.</param>
        /// <param name="formatProvider">The culture-specific formatting information.</param>
        /// <returns>A formatted string representing the MinMaxInt instance.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            format = string.IsNullOrEmpty(format) ? "D" : format;
            formatProvider ??= CultureInfo.InvariantCulture;

            return string.Format(formatProvider, "({0}, {1})", min.ToString(format, formatProvider), max.ToString(format, formatProvider));
        }

        public static MinMaxInt operator +(MinMaxInt a, MinMaxInt b)
        {
            return new MinMaxInt(a.min + b.min, a.max + b.max);
        }

        public static MinMaxInt operator -(MinMaxInt a, MinMaxInt b)
        {
            return new MinMaxInt(a.min - b.min, a.max - b.max);
        }

        public static MinMaxInt operator *(MinMaxInt a, MinMaxInt b)
        {
            return new MinMaxInt(a.min * b.min, a.max * b.max);
        }

        public static MinMaxInt operator /(MinMaxInt a, MinMaxInt b)
        {
            return new MinMaxInt(a.min / b.min, a.max / b.max);
        }

        public static MinMaxInt operator -(MinMaxInt a)
        {
            return new MinMaxInt(-a.min, -a.max);
        }

        public static MinMaxInt operator *(MinMaxInt a, int d)
        {
            return new MinMaxInt(a.min * d, a.max * d);
        }

        public static MinMaxInt operator *(int d, MinMaxInt a)
        {
            return new MinMaxInt(a.min * d, a.max * d);
        }

        public static MinMaxInt operator /(MinMaxInt a, int d)
        {
            return new MinMaxInt(a.min / d, a.max / d);
        }

        public static bool operator ==(MinMaxInt lhs, MinMaxInt rhs)
        {
            return lhs.min == rhs.min && lhs.max == rhs.max;
        }

        public static bool operator !=(MinMaxInt lhs, MinMaxInt rhs)
        {
            return !(lhs == rhs);
        }

        public static implicit operator MinMaxInt(Vector2Int v)
        {
            return new MinMaxInt(v.x, v.y);
        }

        public static implicit operator Vector2Int(MinMaxInt v)
        {
            return new Vector2Int(v.min, v.max);
        }
    }
}