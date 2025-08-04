﻿using System;

namespace Gamegaard.Utils
{
    public static class EnumUtils
    {
        public static T GetRandomEnumValue<T>() where T : Enum
        {
            Array enumValues = Enum.GetValues(typeof(T));
            int randomIndex = UnityEngine.Random.Range(0, enumValues.Length);
            return (T)enumValues.GetValue(randomIndex);
        }

        public static T GetRandomEnumValue<T>(Action<T> condition) where T : Enum
        {
            Array enumValues = Enum.GetValues(typeof(T));
            int randomIndex;
            T randomEnumValue;

            do
            {
                randomIndex = UnityEngine.Random.Range(0, enumValues.Length);
                randomEnumValue = (T)enumValues.GetValue(randomIndex);
            }
            while (!IsValid(randomEnumValue, condition));

            return randomEnumValue;
        }

        private static bool IsValid<T>(T value, Action<T> condition) where T : Enum
        {
            if (condition == null)
            {
                return true;
            }

            condition.Invoke(value);
            return true;
        }

        public static T[] GetRandomEnumValueAmount<T>(int amount) where T : Enum
        {
            T[] enumElements = new T[amount];
            for (int i = 0; i < amount; i++)
            {
                enumElements[i] = GetRandomEnumValue<T>();
            }
            return enumElements;
        }

        public static T[] GetRandomEnumValueAmount<T>(int amount, Action<T> condition) where T : Enum
        {
            T[] enumElements = new T[amount];
            for (int i = 0; i < amount; i++)
            {
                enumElements[i] = GetRandomEnumValue(condition);
            }
            return enumElements;
        }
    }
}