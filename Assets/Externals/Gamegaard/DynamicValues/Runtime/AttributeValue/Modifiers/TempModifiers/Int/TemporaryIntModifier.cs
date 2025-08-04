namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Represents a temporary integer modifier that expires after a specified duration.
    /// </summary>
    public class TemporaryIntModifier : TemporaryModifier<int>
    {
        /// <summary>
        /// Gets a value indicating whether this modifier has a non-zero base value.
        /// </summary>
        public override bool HasValue => baseValue != 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporaryIntModifier"/> class.
        /// </summary>
        /// <param name="type">The calculation type of the modifier.</param>
        /// <param name="value">The base integer value of the modifier.</param>
        /// <param name="durationInSeconds">The duration of the modifier in seconds.</param>
        /// <param name="source">The source object associated with this modifier.</param>
        /// <param name="calculationStrategy">The calculation strategy to apply (optional).</param>
        public TemporaryIntModifier(ModifierCalculationType type, int value, float durationInSeconds, object source, string id = null, ModifierStrategy<int> calculationStrategy = null) : base(type, value, durationInSeconds, source, id, calculationStrategy)
        {
        }

        public TemporaryIntModifier(TemporaryIntModifier other) : base(other.type, other.baseValue, other.durationInSeconds, other.source, other.id, other.calculationStrategy)
        {
        }

        /// <summary>
        /// Initializes a new default instance of the <see cref="TemporaryIntModifier"/> class.
        /// </summary>
        public TemporaryIntModifier() : base(ModifierCalculationType.Flat, 0, 0, null)
        {
        }
    }
}
