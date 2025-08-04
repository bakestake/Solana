namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Represents a modifier that operates on floating-point values.
    /// </summary>
    public class FloatModifier : Modifier<float>
    {
        /// <summary>
        /// Determines whether this modifier has a non-zero value.
        /// </summary>
        public override bool HasValue => baseValue != 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="FloatModifier"/> class.
        /// </summary>
        /// <param name="type">The calculation type of the modifier.</param>
        /// <param name="value">The floating-point value of the modifier.</param>
        /// <param name="source">The source object associated with the modifier.</param>
        /// <param name="calculationStrategy">The calculation strategy to apply (optional).</param>
        public FloatModifier(ModifierCalculationType type, float value, object source, string id = null, ModifierStrategy<float> calculationStrategy = null)
            : base(type, value, source, id, calculationStrategy)
        {
        }
    }
}