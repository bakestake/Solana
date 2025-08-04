using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Manages and applies multiple modifiers to a specified receiver, supporting operations such as addition, removal, and description aggregation.
    /// </summary>
    /// <typeparam name="TCaster">The type of the object that can receive the modifiers.</typeparam>
    [System.Serializable]
    public class MultipleModifierApplier<TCaster> : IModifierApplier<TCaster>
    {
        /// <summary>
        /// The list of all modifiers managed by this applier.
        /// </summary>
        [SerializeReference] private List<ModifierApplierBase> _modifiers = new List<ModifierApplierBase>();

        /// <summary>
        /// Gets a read-only list of modifiers managed by this applier.
        /// </summary>
        public IReadOnlyList<ModifierApplierBase> Modfiers => _modifiers;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleModifierApplier{CasterType}"/> class with an empty list of modifiers.
        /// </summary>
        public MultipleModifierApplier()
        {
            _modifiers = new List<ModifierApplierBase>();
        }

        /// <summary>
        /// Adds a modifier to the applier.
        /// </summary>
        /// <param name="modifierApplierBase">The modifier to add.</param>
        public void AddModfier(ModifierApplierBase modifierApplierBase)
        {
            _modifiers.Add(modifierApplierBase);
        }

        /// <summary>
        /// Removes a specified modifier from the applier.
        /// </summary>
        /// <param name="modifierApplierBase">The modifier to remove.</param>
        /// <returns>True if the modifier was removed; otherwise, false.</returns>
        public bool RemoveModfier(ModifierApplierBase modifierApplierBase)
        {
            return _modifiers.Remove(modifierApplierBase);
        }

        /// <summary>
        /// Checks if the applier contains a specified modifier.
        /// </summary>
        /// <param name="modifierApplierBase">The modifier to check.</param>
        /// <returns>True if the modifier is in the list; otherwise, false.</returns>
        public bool ContaisModfier(ModifierApplierBase modifierApplierBase)
        {
            return _modifiers.Contains(modifierApplierBase);
        }

        /// <summary>
        /// Retrieves a random modifier from the list of modifiers.
        /// </summary>
        /// <returns>A randomly selected modifier.</returns>
        public ModifierApplierBase GetRandom()
        {
            if (_modifiers.Count == 0) return null;
            return _modifiers[Random.Range(0, _modifiers.Count)];
        }

        /// <summary>
        /// Applies all managed modifiers to the specified receiver.
        /// </summary>
        /// <param name="receiver">The object to which the modifiers will be applied.</param>
        public void Apply(TCaster receiver)
        {
            foreach (ModifierApplierBase mod in _modifiers)
            {
                if (mod is ModifierApplierBaseGeneric<TCaster> cMod)
                {
                    cMod.Apply(receiver);
                }
            }
        }

        /// <summary>
        /// Removes all managed modifiers from the specified receiver.
        /// </summary>
        /// <param name="receiver">The object from which the modifiers will be removed.</param>
        public void Remove(TCaster receiver)
        {
            foreach (ModifierApplierBase mod in _modifiers)
            {
                if (mod is ModifierApplierBaseGeneric<TCaster> cMod)
                {
                    cMod.Remove(receiver);
                }
            }
        }

        /// <summary>
        /// Checks if all managed modifiers can affect the specified receiver.
        /// </summary>
        /// <param name="receiver">The object to check for applicability.</param>
        /// <returns>True if all modifiers can affect the receiver; otherwise, false.</returns>
        public bool CanAffect(TCaster receiver)
        {
            foreach (ModifierApplierBase mod in _modifiers)
            {
                if (mod is ModifierApplierBaseGeneric<TCaster> cMod)
                {
                    if (!cMod.CanAffect(receiver)) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets a concatenated description of all managed modifiers.
        /// </summary>
        /// <returns>A string representing the descriptions of all modifiers.</returns>
        public string GetDescription()
        {
            string text = string.Empty;

            foreach (ModifierApplierBase mod in _modifiers)
            {
                if (mod is ModifierApplierBaseGeneric<TCaster> cMod)
                {
                    text += cMod.GetDescription() + "\n";
                }
            }

            return text;
        }
    }
}