using System.Globalization;
using System;

namespace Gamegaard.RuntimeDebug
{
    public class ArrayParser<T> : ITypeParser<T[]>
    {
        public T[] Parse(string value)
        {
            string[] parts = value.Split(',');
            T[] array = new T[parts.Length];

            for (int i = 0; i < parts.Length; i++)
            {
                array[i] = (T)Convert.ChangeType(parts[i].Trim(), typeof(T), CultureInfo.InvariantCulture);
            }

            return array;
        }
    }
}