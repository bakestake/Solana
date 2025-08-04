using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Represents a dynamic integer value that can be modified by temporary and permanent modifiers, with support for complex calculation strategies.
    /// </summary>
    [Serializable]
    public class DynamicInt : DynamicValue<int>
    {
        public override int MaximumValue { get; set; } = int.MaxValue;
        public override int MinimumValue { get; set; } = int.MinValue;

        public DynamicInt() { }

        public DynamicInt(int baseValue) : base(baseValue) { }

        public DynamicInt(DynamicInt other) : base(other.BaseValue) { }

        protected sealed override int CalculateFinalValue<TValue>(List<TemporaryModifier<int>> tempMods, List<Modifier<int>> mods)
        {
            int finalValue = BaseValue;
            float multiplier = 1;

            for (int i = 0; i < mods.Count; i++)
            {
                ProcessModifier(mods[i], ref finalValue, ref multiplier);
            }

            for (int i = 0; i < tempMods.Count; i++)
            {
                ProcessModifier(tempMods[i], ref finalValue, ref multiplier);
            }

            return ClampAndRound(finalValue * multiplier);
        }

        protected sealed override int SumValueWithMode(int value, Modifier<int> mod)
        {
            return value + EvaluateModValue(mod);
        }

        protected sealed override int SubtractValues(int value1, int value2)
        {
            return value1 - value2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ProcessModifier(Modifier<int> mod, ref int finalValue, ref float multiplier)
        {
            if (mod.type == ModifierCalculationType.OverallPercentage)
            {
                multiplier += NormalizedValue(mod.CurrentValue);
            }
            else
            {
                finalValue += EvaluateModValue(mod);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ClampAndRound(float value)
        {
            return Mathf.Clamp(Mathf.RoundToInt(value), MinimumValue, MaximumValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected int EvaluateModValue(Modifier<int> mod)
        {
            switch (mod.type)
            {
                case ModifierCalculationType.Flat:
                    return mod.CurrentValue;
                case ModifierCalculationType.BasePercentage:
                    return Mathf.RoundToInt(BaseValue * NormalizedValue(mod.CurrentValue));
                case ModifierCalculationType.OverallFlatPercentage:
                    return Mathf.RoundToInt((BaseValue + GetAllFlat()) * NormalizedValue(mod.CurrentValue));
                case ModifierCalculationType.Custom:
                    return mod.CalculationStrategy?.Apply(this, mod) ?? 0;
                default:
                    return 0;
            }
        }

        protected override int GetAllFlat()
        {
            int finalValue = 0;
            int count = statModifiers.Count;

            for (int i = 0; i < count; i++)
            {
                Modifier<int> mod = statModifiers[i];

                if (mod.type == ModifierCalculationType.Flat)
                    finalValue += mod.baseValue;
            }

            return finalValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float NormalizedValue(int value)
        {
            return value / 100f;
        }
    }
}