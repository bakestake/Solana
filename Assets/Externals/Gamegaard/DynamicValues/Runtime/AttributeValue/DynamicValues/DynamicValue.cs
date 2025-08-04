using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace Gamegaard.DynamicValues
{
    public enum ModifierFilter
    {
        All,
        PermanentOnly,
        TemporaryOnly
    }

    /// <summary>
    /// Represents a dynamic value with modifiers that affect its calculated final value.
    /// Supports both permanent and temporary modifiers, along with event-driven updates.
    /// </summary>
    /// <typeparam name="TValue">The type of the value, constrained to numeric-compatible types.</typeparam>
    public abstract class DynamicValue<TValue>
    {
#if UNITY_EDITOR
        /// <summary>
        /// Used by property drawer to show extra contents.
        /// </summary>
        [SerializeField, HideInInspector] private bool isExpanded;
        internal bool EditorIsExpanded
        {
            get => isExpanded;
            set => isExpanded = value;
        }
#endif

        [SerializeField] private TValue _baseValue;

        /// <summary>
        /// Read-only collection of permanent modifiers applied to this value.
        /// </summary>
        public readonly ReadOnlyCollection<Modifier<TValue>> StatModifiers;

        /// <summary>
        /// Read-only collection of temporary modifiers applied to this value.
        /// </summary>
        public readonly ReadOnlyCollection<TemporaryModifier<TValue>> TempStatModifiers;

        protected readonly List<Modifier<TValue>> statModifiers;
        protected readonly List<TemporaryModifier<TValue>> tempStatModifiers;
        private readonly List<TemporaryModifier<TValue>> tempStatModsBuffer = new List<TemporaryModifier<TValue>>();
        private readonly List<Modifier<TValue>> statModsBuffer = new List<Modifier<TValue>>();

        /// <summary>
        /// Event triggered when the value changes.
        /// </summary>
        public event Action<DynamicValue<TValue>> OnValueChanged;

        [SerializeField][HideInInspector] protected TValue _calculatedValue;
        [SerializeField][HideInInspector] private bool _isDirty = true;

        /// <summary>
        /// The maximum possible value after applying all positive modifiers.
        /// </summary>
        public abstract TValue MaximumValue { get; set; }

        /// <summary>
        /// The minimum possible value after applying all negative modifiers.
        /// </summary>
        public abstract TValue MinimumValue { get; set; }

        /// <summary>
        /// Indicates if there are any modifiers applied (permanent or temporary).
        /// </summary>
        public bool HasAnyModification => HasConstModification || HasTempModification;

        /// <summary>
        /// Indicates if there are permanent modifiers applied.
        /// </summary>
        public bool HasConstModification => statModifiers.Count > 0;

        /// <summary>
        /// Indicates if there are temporary modifiers applied.
        /// </summary>
        public bool HasTempModification => tempStatModifiers.Count > 0;

        /// <summary>
        /// The count of temporary modifiers applied.
        /// </summary>
        public int TempModifierCount => TempStatModifiers.Count;

        /// <summary>
        /// The count of permanent modifiers applied.
        /// </summary>
        public int StaticModifierCount => StatModifiers.Count;

        /// <summary>
        /// The total count of all modifiers applied (permanent and temporary).
        /// </summary>
        public int AllModifiersCount => StatModifiers.Count + TempStatModifiers.Count;

        /// <summary>
        /// The base value without any modifiers.
        /// </summary>
        public TValue BaseValue
        {
            get => _baseValue;
            protected set => _baseValue = value;
        }

        /// <summary>
        /// The calculated final value after applying all modifiers.
        /// Automatically recalculates if the value is marked as dirty.
        /// </summary>
        public TValue CalculatedValue
        {
            get
            {
                RecalculateIfDirty();
                return _calculatedValue;
            }
        }

        /// <summary>
        /// Indicates whether the value needs to be recalculated.
        /// </summary>
        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                _isDirty = value;
                if (value) CallEvents();
            }
        }

        public DynamicValue()
        {
            _isDirty = true;
            statModifiers = new List<Modifier<TValue>>();
            StatModifiers = statModifiers.AsReadOnly();
            tempStatModifiers = new List<TemporaryModifier<TValue>>();
            TempStatModifiers = tempStatModifiers.AsReadOnly();
        }

        public DynamicValue(TValue baseValue) : this()
        {
            _baseValue = baseValue;
        }

        /// <summary>
        /// Updates all temporary modifiers, removing expired ones.
        /// </summary>
        public void UpdateTempModifiers()
        {
            if (!HasTempModification) return;

            bool hasRemovedValues = false;
            for (int i = tempStatModifiers.Count - 1; i >= 0; i--)
            {
                if (tempStatModifiers[i].CheckedUpdate())
                {
                    tempStatModifiers.RemoveAt(i);
                    hasRemovedValues = true;
                }
            }

            if (hasRemovedValues)
            {
                IsDirty = true;
            }
        }

        /// <summary>
        /// Changes the base value and marks the value as dirty.
        /// </summary>
        /// <param name="newValue">The new base value.</param>
        public virtual void ChangeBaseValue(TValue newValue)
        {
            _baseValue = newValue;
            IsDirty = true;
        }

        /// <summary>
        /// Triggers the value changed event.
        /// </summary>
        public virtual void CallEvents()
        {
            OnValueChanged?.Invoke(this);
        }

        /// <summary>
        /// Checks if the specified modifier is present in the list of permanent or temporary modifiers.
        /// </summary>
        /// <param name="modifier">The modifier to check.</param>
        /// <returns>True if the modifier is present; otherwise, false.</returns>
        public virtual bool ContainsModifier(Modifier<TValue> modifier, ModifierFilter filter = ModifierFilter.All)
        {
            return ModifierExists(mod => mod.Equals(modifier), filter, statModifiers, tempStatModifiers);
        }

        /// <summary>
        /// Checks if a modifier with the specified unique identifier is present.
        /// </summary>
        /// <param name="id">The unique identifier of the modifier to check.</param>
        /// <returns>True if a modifier with the specified ID is present; otherwise, false.</returns>
        public virtual bool ContainsModifier(string id, ModifierFilter filter = ModifierFilter.All)
        {
            return ModifierExists(mod => mod.id == id, filter, statModifiers, tempStatModifiers);
        }

        private bool ModifierExists<T>(Func<T, bool> predicate, ModifierFilter searchType, IEnumerable<T> permanent, IEnumerable<T> temporary)
        {
            return searchType switch
            {
                ModifierFilter.PermanentOnly => permanent.Any(predicate),
                ModifierFilter.TemporaryOnly => temporary.Any(predicate),
                _ => permanent.Any(predicate) || temporary.Any(predicate),
            };
        }

        /// <summary>
        /// Attempts to retrieve a modifier with the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the modifier to retrieve.</param>
        /// <param name="modifier">The retrieved modifier if found; otherwise, null.</param>
        /// <returns>True if the modifier is found; otherwise, false.</returns>
        public bool TryGetModifier<ModType>(string id, out ModType modifier, ModifierFilter filter = ModifierFilter.All) where ModType : Modifier<TValue>
        {
            switch (filter)
            {
                case ModifierFilter.PermanentOnly:
                    modifier = statModifiers.FirstOrDefault(mod => mod.id == id) as ModType;
                    break;
                case ModifierFilter.TemporaryOnly:
                    modifier = tempStatModifiers.FirstOrDefault(mod => mod.id == id) as ModType;
                    break;
                default:
                    modifier = statModifiers.FirstOrDefault(mod => mod.id == id) as ModType;
                    modifier ??= tempStatModifiers.FirstOrDefault(mod => mod.id == id) as ModType;
                    break;
            }

            return modifier != null;
        }

        /// <summary>
        /// Removes a modifier based on its unique identifier.
        /// </summary>
        /// <param name="modifierID">The unique identifier of the modifier to remove.</param>
        /// <returns>True if the modifier was found and removed; otherwise, false.</returns>
        public bool RemoveModifier(string modifierID, ModifierFilter filter = ModifierFilter.All)
        {
            if (filter == ModifierFilter.All || filter == ModifierFilter.PermanentOnly)
            {
                for (int i = statModifiers.Count - 1; i >= 0; i--)
                {
                    if (statModifiers[i].id == modifierID)
                    {
                        statModifiers.RemoveAt(i);
                        IsDirty = true;
                        return true;
                    }
                }
            }

            if (filter == ModifierFilter.All || filter == ModifierFilter.TemporaryOnly)
            {
                for (int i = tempStatModifiers.Count - 1; i >= 0; i--)
                {
                    if (tempStatModifiers[i].id == modifierID)
                    {
                        tempStatModifiers.RemoveAt(i);
                        IsDirty = true;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Retrieves all modifiers (permanent and temporary).
        /// </summary>
        /// <returns>A collection containing all applied modifiers.</returns>
        public IEnumerable<Modifier<TValue>> GetAllModifiers(ModifierFilter filter = ModifierFilter.All)
        {
            if (filter == ModifierFilter.PermanentOnly)
            {
                foreach (Modifier<TValue> modifier in statModifiers)
                {
                    yield return modifier;
                }
            }

            if (filter == ModifierFilter.TemporaryOnly)
            {
                foreach (TemporaryModifier<TValue> tempModifier in tempStatModifiers)
                {
                    yield return tempModifier;
                }
            }
        }

        /// <summary>
        /// Retrieves all modifiers from a specific source.
        /// </summary>
        /// <param name="source">The source to filter modifiers by.</param>
        /// <returns>A list of modifiers from the specified source.</returns>
        public IEnumerable<Modifier<TValue>> GetModifiersFromSource(object source, ModifierFilter filter = ModifierFilter.All)
        {
            if (filter == ModifierFilter.PermanentOnly)
            {
                foreach (Modifier<TValue> modifier in statModifiers.Where(mod => mod.source == source))
                {
                    yield return modifier;
                }
            }

            if (filter == ModifierFilter.TemporaryOnly)
            {
                foreach (TemporaryModifier<TValue> tempModifier in tempStatModifiers.Where(mod => mod.source == source))
                {
                    yield return tempModifier;
                }
            }
        }

        /// <summary>
        /// Filters temporary and permanent modifiers based on a custom condition.
        /// </summary>
        /// <param name="predicate">The condition to filter modifiers by.</param>
        /// <returns>A list of modifiers that match the condition.</returns>
        public IEnumerable<Modifier<TValue>> FilterModifiers(Func<Modifier<TValue>, bool> predicate)
        {
            return statModifiers.Where(predicate).Concat(tempStatModifiers.Where(predicate));
        }

        /// <summary>
        /// Adds a new modifier to the value.
        /// </summary>
        public virtual void AddModifier(Modifier<TValue> modifier)
        {
            if (modifier != null && modifier.HasValue)
            {
                if (modifier is TemporaryModifier<TValue> tempMod)
                {
                    tempStatModifiers.Add(tempMod);
                }
                else
                {
                    statModifiers.Add(modifier);
                }
                IsDirty = true;
            }
        }

        /// <summary>
        /// Adds a collection of modifiers to the value.
        /// </summary>
        public virtual void AddModifiers(IEnumerable<Modifier<TValue>> modifiers)
        {
            foreach (Modifier<TValue> modifier in modifiers)
            {
                AddModifier(modifier);
            }
        }

        /// <summary>
        /// Removes a specific modifier from the value.
        /// </summary>
        public bool RemoveModifier(Modifier<TValue> modifier)
        {
            if (statModifiers.Remove(modifier))
            {
                IsDirty = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes all modifiers from a specific source.
        /// </summary>
        public bool RemoveAllModifierFromSource(object source)
        {
            bool hadRemoved = false;
            for (int i = statModifiers.Count - 1; i >= 0; i--)
            {
                if (statModifiers[i].source == source)
                {
                    statModifiers.RemoveAt(i);
                    hadRemoved = true;
                }
            }

            if (hadRemoved)
            {
                IsDirty = true;
            }

            return hadRemoved;
        }

        /// <summary>
        /// Removes a specific temporary modifier from the value.
        /// </summary>
        public bool RemoveTempModifier(TemporaryModifier<TValue> modifier)
        {
            if (tempStatModifiers.Remove(modifier))
            {
                IsDirty = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes all temporary modifiers from a specific source.
        /// </summary>
        public bool RemoveAllTempModifierFromSource(object source)
        {
            bool hadRemoved = false;
            for (int i = tempStatModifiers.Count - 1; i >= 0; i--)
            {
                TemporaryModifier<TValue> mod = tempStatModifiers[i];
                if (mod.source == source)
                {
                    tempStatModifiers.RemoveAt(i);
                    hadRemoved = true;
                    IsDirty = true;
                }
            }
            return hadRemoved;
        }

        /// <summary>
        /// Forces the recalculation of the value if it is dirty.
        /// </summary>
        public virtual void RecalculateIfDirty()
        {
            if (!_isDirty) return;
            Recalculate();
        }

        /// <summary>
        /// Forces the recalculation of the value.
        /// </summary>
        public virtual void Recalculate()
        {
            _calculatedValue = CalculateFinalValue<Modifier<TValue>>(tempStatModifiers, statModifiers);
            IsDirty = false;
            OnCalculatedValueChanged();
        }

        /// <summary>
        /// Optional internal feedback on calculated value changes.
        /// </summary>
        protected virtual void OnCalculatedValueChanged()
        {

        }

        /// <summary>
        /// Calculates the current value provided by a specific modifier.
        /// </summary>
        /// <param name="modifier">The modifier whose current value will be calculated.</param>
        /// <returns>The numerical value contributed by the modifier at this moment.</returns>
        public TValue PreviewModfierValue(Modifier<TValue> modifier)
        {
            int statModifiersCount = statModifiers.Count;
            List<Modifier<TValue>> filteredModifiers = new List<Modifier<TValue>>(statModifiersCount);
            for (int i = 0; i < statModifiersCount; i++)
            {
                Modifier<TValue> mod = statModifiers[i];
                if (!EqualityComparer<Modifier<TValue>>.Default.Equals(mod, modifier))
                {
                    filteredModifiers.Add(mod);
                }
            }

            TValue valueBefore = CalculateFinalValue<Modifier<TValue>>(tempStatModifiers, filteredModifiers);
            TValue valueAfter = SumValueWithMode(valueBefore, modifier);

            return SubtractValues(valueAfter, valueBefore);
        }

        /// <summary>
        /// Calculates and returns the value without saving it. Useful for previewing changes.
        /// </summary>
        public TValue PreviewModifierResult(IEnumerable<Modifier<TValue>> modifiers)
        {
            return PreviewModifierResultInternal(modifiers);
        }

        /// <summary>
        /// Calculates and returns the value without saving it. Useful for previewing a single modifier's effect.
        /// </summary>
        public TValue PreviewModifierResult(Modifier<TValue> modifier)
        {
            return PreviewModifierResultInternal(new[] { modifier });
        }

        private TValue PreviewModifierResultInternal(IEnumerable<Modifier<TValue>> modifiers)
        {
            tempStatModsBuffer.Clear();
            tempStatModsBuffer.AddRange(tempStatModifiers);

            statModsBuffer.Clear();
            statModsBuffer.AddRange(statModifiers);

            foreach (Modifier<TValue> mod in modifiers)
            {
                if (mod is TemporaryModifier<TValue> tempMod)
                {
                    tempStatModsBuffer.Add(tempMod);
                }
                else
                {
                    statModsBuffer.Add(mod);
                }
            }

            return CalculateFinalValue<Modifier<TValue>>(tempStatModsBuffer, statModsBuffer);
        }

        /// <summary>
        /// Calculate the final value considering all modifiers.
        /// </summary>
        protected abstract TValue CalculateFinalValue<H>(List<TemporaryModifier<TValue>> tempMods, List<Modifier<TValue>> mods) where H : Modifier<TValue>;

        /// <summary>
        /// Get the total value of flat modifiers.
        /// </summary>
        protected abstract TValue GetAllFlat();

        /// <summary>
        /// Sum a modifier to a base value.
        /// </summary>
        protected abstract TValue SumValueWithMode(TValue value, Modifier<TValue> mod);

        /// <summary>
        /// Subtracts two values of type ValueType. This should be implemented to support specific type calculations.
        /// </summary>
        /// <param name="value1">The first value.</param>
        /// <param name="value2">The second value.</param>
        /// <returns>The result of subtracting value2 from value1.</returns>
        protected abstract TValue SubtractValues(TValue value1, TValue value2);

        /// <summary>
        /// Adds all modifiers to the base value and optionally removes them. Filters can be applied.
        /// </summary>
        public void AbsorbModifiersIntoBaseValue(Func<Modifier<TValue>, bool> filter = null)
        {
            TValue newBaseValue = _baseValue;
            for (int i = 0; i < statModifiers.Count; i++)
            {
                Modifier<TValue> mod = statModifiers[i];
                if (filter == null || filter(mod))
                {
                    newBaseValue = SumValueWithMode(newBaseValue, mod);
                }
            }

            ChangeBaseValue(newBaseValue);

            if (filter != null)
            {
                statModifiers.RemoveAll(mod => filter(mod));
            }
            else
            {
                statModifiers.Clear();
            }

            IsDirty = true;
        }

        /// <summary>
        /// Normalizes a value by dividing it by 100.
        /// </summary>
        /// <param name="value">The value to be normalized.</param>
        /// <returns>The normalized value as a fraction of 1.</returns>
        protected float NormalizedValue(float value)
        {
            return value / 100f;
        }

        public override string ToString()
        {
            return CalculatedValue.ToString();
        }
    }
}