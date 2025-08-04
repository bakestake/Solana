using System;

namespace Gamegaard
{
    /// <summary>
    /// Represents a value range with both upper and lower bounds, ensuring the value stays within the defined range.
    /// Extends <see cref="IBoundedValue{T}"/> by adding a minimum value.
    /// </summary>
    /// <typeparam name="T">The type of the clamped range, which must implement common numeric and comparable interfaces.</typeparam>
    public interface IClampedRange<T> : IBoundedValue<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        /// <summary>
        /// Gets the minimum allowable value.
        /// </summary>
        T MinValue { get; }
    }
}
