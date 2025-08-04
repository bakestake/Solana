using System;
using UnityEngine;

namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Base class for applying modifiers of a specific type, providing functionality for defining, creating, and managing modifiers.
    /// </summary>
    /// <typeparam name="TTarget">The type of the object that will receive the modifier.</typeparam>
    /// <typeparam name="TValue">The type of the value associated with the modifier, constrained to numeric-compatible types.</typeparam>
    [Serializable]
    public abstract class ModifierApplier<TTarget, TValue> : ModifierApplierBaseGeneric<TTarget>, IModifierApplier<TTarget, TValue>
    {
        /// <summary>
        /// Defines the type of calculation applied by the modifier.
        /// </summary>
        [Tooltip("Defines the type of modifier to apply.")]
        [SerializeField] protected ModifierCalculationType type;

        /// <summary>
        /// The value applied by the modifier.
        /// </summary>
        [Tooltip("The value applied by the modifier.")]
        [SerializeField] protected TValue value;

        /// <summary>
        /// The duration of the modifier in seconds. Use 0 for permanent effects.
        /// </summary>
        [Min(0)]
        [Tooltip("The duration of the modifier in seconds. Use 0 for permanent effects.")]
        [SerializeField] protected float durationInSeconds;

        /// <summary>
        /// The calculation strategy used by the modifier, if applicable.
        /// </summary>
        [SerializeReference] protected ModifierStrategyBase calculationStrategy;

        /// <summary>
        /// Gets or sets the type of calculation applied by the modifier.
        /// </summary>
        public ModifierCalculationType ModfierType { get => type; set => type = value; }

        /// <summary>
        /// Gets or sets the value of the modifier.
        /// </summary>
        public TValue Value { get => value; set => this.value = value; }

        /// <summary>
        /// Gets the duration of the modifier in seconds.
        /// </summary>
        public float DurationInSeconds => durationInSeconds;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifierApplier{ModifierBase, CasterType, ValueType}"/> class.
        /// </summary>
        public ModifierApplier() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifierApplier{ModifierBase, CasterType, ValueType}"/> class with specified parameters.
        /// </summary>
        /// <param name="type">The type of calculation applied by the modifier.</param>
        /// <param name="value">The value applied by the modifier.</param>
        /// <param name="durationInSeconds">The duration of the modifier in seconds. Default is 0 for permanent effects.</param>
        public ModifierApplier(ModifierCalculationType type, TValue value, float durationInSeconds = 0)
        {
            this.type = type;
            this.value = value;
            this.durationInSeconds = durationInSeconds;
        }

        /// <summary>
        /// Sets the duration of the modifier in seconds.
        /// </summary>
        /// <param name="durationInSeconds">The new duration of the modifier.</param>
        public void SetDuration(float durationInSeconds)
        {
            this.durationInSeconds = durationInSeconds;
        }
    }
}