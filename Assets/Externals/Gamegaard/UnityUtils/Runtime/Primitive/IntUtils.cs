﻿using UnityEngine;

namespace Gamegaard.Utils
{
    public static class IntUtils
    {
        /// <summary>
        /// Retorna um valor igual a 1 ou -1.
        /// </summary>
        public static int GetRandomSign()
        {
            return Random.value < .5 ? 1 : -1;
        }

        /// <summary>
        /// Retorna o valor limitado a 0.
        /// </summary>
        public static int ClampIntToZero(int value)
        {
            return (int)Mathf.Clamp(value, 0, float.MaxValue);
        }

        /// <summary>
        /// Retorna o valor com o sinal (positivo ou negativo) de forma randominca.
        /// </summary>
        public static int RandomizeSing(this int value)
        {
            return value *= GetRandomSign();
        }

        /// <summary>
        /// Retorna em porcentagem o quanto o valor atual vale entre um valor minimo e maximo.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static float PercentBtweenValues(int value, int minValue, int maxValue)
        {
            return (value - (float)minValue) / (maxValue - (float)minValue);
        }
    }
}