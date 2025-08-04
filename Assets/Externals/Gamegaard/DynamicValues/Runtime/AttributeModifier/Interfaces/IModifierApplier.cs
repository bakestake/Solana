using System;

namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Defines methods for applying and removing modifiers to a receiver, with additional checks and descriptions.
    /// </summary>
    /// <typeparam name="TTarget">The type of the object that will be affected by the modifier.</typeparam>
    public interface IModifierApplier<TTarget>
    {
        /// <summary>
        /// Applies the modifier to the specified receiver.
        /// </summary>
        /// <param name="receiver">The receiver to which the modifier will be applied.</param>
        void Apply(TTarget receiver);

        /// <summary>
        /// Removes the modifier from the specified receiver.
        /// </summary>
        /// <param name="receiver">The receiver from which the modifier will be removed.</param>
        void Remove(TTarget receiver);

        /// <summary>
        /// Determines whether the modifier can affect the specified receiver.
        /// </summary>
        /// <param name="receiver">The receiver to check for applicability.</param>
        /// <returns>True if the modifier can affect the receiver; otherwise, false.</returns>
        bool CanAffect(TTarget receiver);

        /// <summary>
        /// Gets a description of the modifier.
        /// </summary>
        /// <returns>A string describing the modifier.</returns>
        string GetDescription();
    }

    /// <summary>
    /// Extends <see cref="IModifierApplier{TTarget}"/> to include a value property for numeric-based modifiers.
    /// </summary>
    /// <typeparam name="TTarget">The type of the object that will be affected by the modifier.</typeparam>
    /// <typeparam name="ValueType">The type of the value associated with the modifier, constrained to numeric-compatible types.</typeparam>
    public interface IModifierApplier<TTarget, ValueType>
    {
        /// <summary>
        /// Gets or sets the value of the modifier.
        /// </summary>
        ValueType Value { get; set; }
    }
}
