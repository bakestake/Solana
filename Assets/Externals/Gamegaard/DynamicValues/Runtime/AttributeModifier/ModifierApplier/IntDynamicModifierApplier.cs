namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// A base class for applying integer modifiers to a specific type of caster.
    /// </summary>
    /// <typeparam name="TCaster">The type of the object that will receive the integer modifiers.</typeparam>
    [System.Serializable]
    public abstract class IntDynamicModifierApplier<TCaster> : DynamicModifierApplier<Modifier<int>, TCaster, int>
    {
        public override Modifier<int> CreateModifierInstance(object source, string id = null, float durationInSeconds = 0)
        {
            Modifier<int> mod;
            if (durationInSeconds == 0)
            {
                mod = new IntModifier(type, value, source, id, CalculationStrategy);
            }
            else
            {
                mod = new TemporaryIntModifier(type, value, durationInSeconds, source, id, CalculationStrategy);
            }
            return mod;
        }
    }
}