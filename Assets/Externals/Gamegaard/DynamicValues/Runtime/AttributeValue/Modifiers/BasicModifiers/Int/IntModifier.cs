namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Represents a modifier that operates on integer values.
    /// </summary>
    public class IntModifier : Modifier<int>
    {
        /// <summary>
        /// Determines whether this modifier has a non-zero value.
        /// </summary>
        public override bool HasValue => baseValue != 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntModifier"/> class.
        /// </summary>
        /// <param name="type">The calculation type of the modifier.</param>
        /// <param name="value">The integer value of the modifier.</param>
        /// <param name="source">The source object associated with the modifier.</param>
        /// <param name="calculationStrategy">The calculation strategy to apply (optional).</param>
        public IntModifier(ModifierCalculationType type, int value, object source, string id = null, ModifierStrategy<int> calculationStrategy = null)
            : base(type, value, source, id, calculationStrategy)
        {
        }
    }
}