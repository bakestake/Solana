using System;

namespace Gamegaard
{
    /// <summary>
    /// Represents a value with defined bounds, including a maximum value and a current value.
    /// Commonly used for attributes like Health, Mana, or Energy where the value must stay within a specified range.
    /// </summary>
    /// <typeparam name="T">The type of the bounded value, which must implement common numeric and comparable interfaces.</typeparam>
    public interface IBoundedValue<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        /// <summary>
        /// Gets the maximum allowable value.
        /// </summary>
        T MaxValue { get; }

        /// <summary>
        /// Gets the current value within the bounds.
        /// </summary>
        T CurrentValue { get; }
    }
}
