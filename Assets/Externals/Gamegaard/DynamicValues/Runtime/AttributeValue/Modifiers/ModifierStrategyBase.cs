using System;

namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Represents the base class for a calculation strategy used by modifiers.
    /// </summary>
    [Serializable]
    public class ModifierStrategyBase
    {
        /// <summary>
        /// Gets a description of the calculation strategy.
        /// </summary>
        public virtual string Description => "";
    }
}