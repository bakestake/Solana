namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Base class for applying modifiers of a specific type, providing functionality for defining, creating, and managing modifiers.
    /// </summary>
    /// <typeparam name="TModifier">The base type of the modifier to be applied.</typeparam>
    /// <typeparam name="TTarget">The type of the object that will receive the modifier.</typeparam>
    /// <typeparam name="TValue">The type of the value associated with the modifier, constrained to numeric-compatible types.</typeparam>
    public abstract class DynamicModifierApplier<TModifier, TTarget, TValue> : ModifierApplier<TTarget, TValue> where TModifier : Modifier<TValue>
    {
        /// <summary>
        /// Gets the calculation strategy used for custom modifiers.
        /// </summary>
        protected ModifierStrategy<TValue> CalculationStrategy => type == ModifierCalculationType.Custom ? calculationStrategy as ModifierStrategy<TValue> : null;

        /// <summary>
        /// Creates an instance of the specified modifier.
        /// </summary>
        /// <param name="source">The source object associated with the modifier.</param>
        /// <param name="durationInSeconds">The duration of the modifier in seconds. Default is 0 for permanent effects.</param>
        /// <returns>An instance of the specified modifier type.</returns>
        public abstract TModifier CreateModifierInstance(object source, string id = null, float durationInSeconds = 0);
    }
}