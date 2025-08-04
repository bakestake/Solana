using UnityEngine;

namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// A temporary integer modifier that adjusts its value over time using an <see cref="AnimationCurve"/>.
    /// </summary>
    public class TemporaryIntModifierWithCurve : TemporaryIntModifier
    {
        /// <summary>
        /// The animation curve used to adjust the modifier's value over time.
        /// </summary>
        [SerializeField] private AnimationCurve valueCurve;

        /// <summary>
        /// Gets the current value of the modifier, adjusted based on the timer's progress and the animation curve.
        /// </summary>
        public override int CurrentValue => (int)(baseValue * valueCurve.Evaluate(Percentage));

        /// <summary>
        /// Gets the animationCurve used to adjust the modifier's value over time.
        /// </summary>
        public AnimationCurve ValueCurve => valueCurve;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporaryIntModifierWithCurve"/> class.
        /// </summary>
        /// <param name="type">The calculation type of the modifier.</param>
        /// <param name="value">The base value of the modifier.</param>
        /// <param name="durationInSeconds">The duration of the modifier in seconds.</param>
        /// <param name="source">The source object associated with the modifier.</param>
        /// <param name="curve">The animation curve used to adjust the value over time.</param>
        /// <param name="calculationStrategy">The calculation strategy to apply (optional).</param>
        public TemporaryIntModifierWithCurve(ModifierCalculationType type, int value, float durationInSeconds, object source, AnimationCurve curve, string id = null, ModifierStrategy<int> calculationStrategy = null) : base(type, value, durationInSeconds, source, id, calculationStrategy)
        {
            valueCurve = curve;
        }

        public TemporaryIntModifierWithCurve(TemporaryIntModifierWithCurve other) : this(other.type, other.baseValue, other.durationInSeconds, other.source, new AnimationCurve(other.valueCurve.keys), other.id, other.calculationStrategy)
        {
        }

        /// <summary>
        /// Initializes a new default instance of the <see cref="TemporaryIntModifierWithCurve"/> class.
        /// </summary>
        public TemporaryIntModifierWithCurve() : base()
        {
            valueCurve = AnimationCurve.Constant(0, 1, 1);
        }

        public void SetCurve(AnimationCurve newCurve)
        {
            valueCurve = newCurve;
        }
    }
}