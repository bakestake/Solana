using UnityEngine;

namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Represents a value modifier that alters a dynamic value using a specific calculation strategy.
    /// </summary>
    /// <typeparam name="TValue">The type of the value to be modified, constrained to numeric-compatible types.</typeparam>
    public abstract class Modifier<TValue> : ModifierBase
    {
        /// <summary>
        /// The calculation strategy used to apply the modifier's effect.
        /// </summary>
        [SerializeField] protected ModifierStrategy<TValue> calculationStrategy;

        /// <summary>
        /// The base value of the modifier, before applying any calculations.
        /// </summary>
        public readonly TValue baseValue;

        /// <summary>
        /// Gets the current value of the modifier, typically equal to the base value.
        /// </summary>
        public virtual TValue CurrentValue => baseValue;

        /// <summary>
        /// Gets the calculation strategy associated with this modifier.
        /// </summary>
        public ModifierStrategy<TValue> CalculationStrategy => calculationStrategy;

        /// <summary>
        /// Sets a new calculation strategy for this modifier.
        /// </summary>
        /// <param name="calculationStrategy">The new calculation strategy to assign.</param>
        public void SetStrategy(ModifierStrategy<TValue> calculationStrategy)
        {
            this.calculationStrategy = calculationStrategy;
        }

        /// <summary>
        /// Initializes a new instance of the Modifier class.
        /// </summary>
        /// <param name="type">The calculation type of the modifier.</param>
        /// <param name="value">The base value of the modifier.</param>
        /// <param name="source">The source object associated with the modifier.</param>
        /// <param name="calculationStrategy">The calculation strategy to use (optional).</param>
        public Modifier(ModifierCalculationType type, TValue value, object source, string id = null, ModifierStrategy<TValue> calculationStrategy = null) : base(type, source, id)
        {
            baseValue = value;
            this.calculationStrategy = calculationStrategy;
        }

        public Modifier(Modifier<TValue> other) : this(other.type, other.baseValue, other.source, other.id, other.calculationStrategy) 
        {
        }

        /// <summary>
        /// Gets the current value of the modifier as a generic object.
        /// </summary>
        /// <returns>The current value of the modifier.</returns>
        public override object GetValue() => CurrentValue;
    }
}