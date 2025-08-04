using System.Globalization;
using System;
using System.Collections.Generic;

namespace Gamegaard.RuntimeDebug
{
    public class CollectionParser<T> : ITypeParser<List<T>>
    {
        public List<T> Parse(string value)
        {
            string[] parts = value.Split(',');
            List<T> list = new List<T>();

            foreach (string part in parts)
            {
                list.Add((T)Convert.ChangeType(part.Trim(), typeof(T), CultureInfo.InvariantCulture));
            }

            return list;
        }
    }
}