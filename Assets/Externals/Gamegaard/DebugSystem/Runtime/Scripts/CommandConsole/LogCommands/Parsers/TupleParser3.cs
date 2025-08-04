using System.Globalization;
using System;

namespace Gamegaard.RuntimeDebug
{
    public class TupleParser3<T1, T2, T3> : ITypeParser<(T1, T2, T3)>
    {
        public (T1, T2, T3) Parse(string value)
        {
            string[] parts = value.Split(',');

            if (parts.Length != 3)
                throw new FormatException($"Input '{value}' cannot be parsed into a tuple of 3 elements.");

            T1 item1 = (T1)Convert.ChangeType(parts[0].Trim(), typeof(T1), CultureInfo.InvariantCulture);
            T2 item2 = (T2)Convert.ChangeType(parts[1].Trim(), typeof(T2), CultureInfo.InvariantCulture);
            T3 item3 = (T3)Convert.ChangeType(parts[2].Trim(), typeof(T3), CultureInfo.InvariantCulture);

            return (item1, item2, item3);
        }
    }
}