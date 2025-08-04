using System;
using System.Collections.Generic;
using System.Linq;

namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Represents a calculation strategy for applying a modifier to a dynamic value.
    /// </summary>
    /// <typeparam name="TValue">The type of value being modified, constrained to numeric-compatible types.</typeparam>
    [Serializable]
    public abstract class ModifierStrategy<TValue> : ModifierStrategyBase
    {
        /// <summary>
        /// Applies the calculation logic for a modifier based on the given dynamic value and the modifier's properties.
        /// </summary>
        /// <param name="dynamicValue">The dynamic value being modified.</param>
        /// <param name="modifier">The modifier to be applied.</param>
        /// <returns>The result of the calculation as a value of type <typeparamref name="TValue"/>.</returns>
        public abstract TValue Apply(DynamicValue<TValue> dynamicValue, Modifier<TValue> modfier);

        /// <summary>
        /// Normalizes a value by dividing it by 100.
        /// </summary>
        /// <param name="value">The value to be normalized.</param>
        /// <returns>The normalized value as a fraction of 1.</returns>
        protected float NormalizedValue(float value)
        {
            return value / 100f;
        }

        /// <summary>
        /// Retrieves all modifiers of a specific calculation type from the given dynamic value.
        /// </summary>
        /// <param name="dynamicValue">The dynamic value containing the list of modifiers.</param>
        /// <param name="type">The calculation type to filter modifiers by.</param>
        /// <returns>A list of modifiers matching the specified calculation type.</returns>
        protected List<Modifier<TValue>> GetModifiersByType(DynamicValue<TValue> dynamicValue, ModifierCalculationType type)
        {
            return dynamicValue.StatModifiers.Where(x => x.type == type).ToList();
        }
    }
}