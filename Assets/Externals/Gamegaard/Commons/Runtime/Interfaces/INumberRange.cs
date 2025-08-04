using System;

namespace Gamegaard
{
    /// <summary>
    /// Represents a numerical range with clamping, percentage calculations, and utility methods for managing the range.
    /// Extends <see cref="IClampedRange{T}"/> by adding advanced functionalities such as percentages and state checks.
    /// </summary>
    /// <typeparam name="T">The type of the numerical range, which must implement common numeric and comparable interfaces.</typeparam>
    public interface INumberRange<T> : IClampedRange<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        /// <summary>
        /// Returns the current value as a percentage of the maximum value.
        /// </summary>
        float Percentage { get; }

        /// <summary>
        /// Returns the inverse percentage of the current value (1 means empty, 0 means full).
        /// </summary>
        float InversePercentage { get; }

        /// <summary>
        /// Returns true if the current value is equal to the maximum value.
        /// </summary>
        bool IsFull { get; }

        /// <summary>
        /// Returns true if the current value is greater than zero.
        /// </summary>
        bool HasValue { get; }

        /// <summary>
        /// Returns true if the current value is equal to zero.
        /// </summary>
        bool IsZero { get; }

        /// <summary>
        /// Returns the amount missing to reach the maximum value.
        /// </summary>
        T MissingValue { get; }

        /// <summary>
        /// Sets the current value to the maximum value.
        /// </summary>
        void SetToMaxValue();

        /// <summary>
        /// Sets the current value to the minimum value.
        /// </summary>
        void SetToMinValue();

        /// <summary>
        /// Sets the current value to a specific percentage of the max value.
        /// </summary>
        void SetToPercentage(float percentage);
    }
}