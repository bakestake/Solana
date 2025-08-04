namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// A base class for applying floating-point modifiers to a specific type of caster.
    /// </summary>
    /// <typeparam name="TTarget">The type of the object that will receive the floating-point modifiers.</typeparam>
    [System.Serializable]
    public abstract class FloatDynamicModifierApplier<TTarget> : DynamicModifierApplier<Modifier<float>, TTarget, float>
    {
        public override string GetDescription()
        {
            return "";
        }

        public override Modifier<float> CreateModifierInstance(object source, string id = null, float durationInSeconds = 0)
        {
            Modifier<float> mod;
            if (durationInSeconds == 0)
            {
                mod = new FloatModifier(type, value, source, id, CalculationStrategy);
            }
            else
            {
                mod = new TemporaryFloatModifier(type, value, durationInSeconds, source, id, CalculationStrategy);
            }
            return mod;
        }
    }
}