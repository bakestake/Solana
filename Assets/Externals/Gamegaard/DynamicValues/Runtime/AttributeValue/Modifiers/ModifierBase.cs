namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Base class for value modifiers, representing a generic modifier with a type and a source.
    /// </summary>
    public abstract class ModifierBase
    {
        /// <summary>
        /// The modifier id.
        /// </summary>
        public readonly string id;

        /// <summary>
        /// The type of calculation applied by this modifier.
        /// </summary>
        public readonly ModifierCalculationType type;

        /// <summary>
        /// The source object associated with this modifier (e.g., an item, ability, or effect).
        /// </summary>
        public readonly object source;

        /// <summary>
        /// Gets a value indicating whether this modifier has a valid, non-default value.
        /// </summary>
        public abstract bool HasValue { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifierBase"/> class.
        /// </summary>
        /// <param name="type">The type of calculation applied by the modifier.</param>
        /// <param name="source">The source object associated with this modifier.</param>
        protected ModifierBase(ModifierCalculationType type, object source, string id)
        {
            this.type = type;
            this.source = source;
            this.id = id;
        }

        protected ModifierBase(ModifierBase other) : this(other.type, other.source, other.id) 
        {
        }

        /// <summary>
        /// Retrieves the current value of the modifier as a generic object.
        /// </summary>
        /// <returns>The current value of the modifier.</returns>
        public abstract object GetValue();
    }
}
