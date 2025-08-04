namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Classe base para aplicadores de modificadores. Necessario pela serialização limitada da Unity quanto a interfaces.
    /// </summary>
    /// <typeparam name="TTarget">Quem é afetado</typeparam>
    public abstract class ModifierApplierBaseGeneric<TTarget> : ModifierApplierBase, IModifierApplier<TTarget>
    {
        public abstract void Apply(TTarget target);
        public abstract void Remove(TTarget target);
        public abstract bool CanAffect(TTarget target);
    }
}