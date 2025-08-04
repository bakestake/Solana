namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Defines the types of calculations that can be applied by a modifier.
    /// </summary>
    public enum ModifierCalculationType
    {
        /// <summary>
        /// Adds a fixed value to the base value.
        /// </summary>
        Flat,

        /// <summary>
        /// Applies a percentage based on the base value.
        /// </summary>
        BasePercentage,

        /// <summary>
        /// Applies a percentage based on the final calculated value, including all modifiers.
        /// </summary>
        OverallPercentage,

        /// <summary>
        /// Applies a percentage to the sum of the base value and flat modifiers, ignoring other percentages.
        /// </summary>
        OverallFlatPercentage,

        /// <summary>
        /// Uses a custom calculation defined by a custom strategy or logic.
        /// </summary>
        Custom
    }
}
