using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Gamegaard.DynamicValues
{
    /// <summary>
    /// Represents a dynamic floating-point value that can be modified by temporary and permanent modifiers, with support for complex calculation strategies.
    /// </summary>
    [Serializable]
    public class DynamicFloat : DynamicValue<float>
    {
        public override float MaximumValue { get; set; } = float.MaxValue;
        public override float MinimumValue { get; set; } = float.MinValue;

        public DynamicFloat()
        {
        }

        public DynamicFloat(float baseValue) : base(baseValue)
        {
        }

        public DynamicFloat(DynamicFloat other) : base(other.BaseValue)
        {
        }

        protected sealed override float CalculateFinalValue<TValue>(List<TemporaryModifier<float>> tempMods, List<Modifier<float>> mods)
        {
            float finalValue = BaseValue;
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

        protected sealed override float SumValueWithMode(float value, Modifier<float> mod)
        {
            return value + EvaluateModValue(mod);
        }

        protected sealed override float SubtractValues(float value1, float value2)
        {
            return value1 - value2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ProcessModifier(Modifier<float> mod, ref float finalValue, ref float multiplier)
        {
            if (mod.type == ModifierCalculationType.OverallPercentage)
            {
                multiplier += mod.CurrentValue;
            }
            else
            {
                finalValue += EvaluateModValue(mod);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float ClampAndRound(float value)
        {
            return Math.Clamp(value, MinimumValue, MaximumValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected float EvaluateModValue(Modifier<float> mod)
        {
            switch (mod.type)
            {
                case ModifierCalculationType.Flat:
                    return mod.CurrentValue;
                case ModifierCalculationType.BasePercentage:
                    return BaseValue * NormalizedValue(mod.CurrentValue);
                case ModifierCalculationType.OverallFlatPercentage:
                    return (BaseValue + GetAllFlat()) * NormalizedValue(mod.CurrentValue);
                case ModifierCalculationType.Custom:
                    return mod.CalculationStrategy?.Apply(this, mod) ?? 0;
                default:
                    return 0;
            }
        }

        protected sealed override float GetAllFlat()
        {
            float finalValue = 0;
            int count = statModifiers.Count;

            for (int i = 0; i < count; i++)
            {
                Modifier<float> mod = statModifiers[i];

                if (mod.type == ModifierCalculationType.Flat)
                    finalValue += mod.baseValue;
            }

            return finalValue;
        }
    }
}