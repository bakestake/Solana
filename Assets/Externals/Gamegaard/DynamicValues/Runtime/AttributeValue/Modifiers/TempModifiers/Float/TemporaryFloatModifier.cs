namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Represents a temporary floating-point modifier that expires after a specified duration.
    /// </summary>
    public class TemporaryFloatModifier : TemporaryModifier<float>
    {
        /// <summary>
        /// Gets a value indicating whether this modifier has a non-zero base value.
        /// </summary>
        public override bool HasValue => baseValue != 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporaryFloatModifier"/> class.
        /// </summary>
        /// <param name="type">The calculation type of the modifier.</param>
        /// <param name="value">The base floating-point value of the modifier.</param>
        /// <param name="durationInSeconds">The duration of the modifier in seconds.</param>
        /// <param name="source">The source object associated with the modifier.</param>
        /// <param name="calculationStrategy">The calculation strategy to apply (optional).</param>
        public TemporaryFloatModifier(ModifierCalculationType type, float value, float durationInSeconds, object source, string id = null, ModifierStrategy<float> calculationStrategy = null) : base(type, value, durationInSeconds, source, id, calculationStrategy)
        {
        }

        public TemporaryFloatModifier(TemporaryFloatModifier other) : base(other.type, other.baseValue, other.durationInSeconds, other.source, other.id, other.calculationStrategy)
        {
        }

        public TemporaryFloatModifier() : base(ModifierCalculationType.Flat, 0, 0, null)
        {
        }
    }
}
