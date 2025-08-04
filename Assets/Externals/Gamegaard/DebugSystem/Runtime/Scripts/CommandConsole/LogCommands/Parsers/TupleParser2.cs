using System.Globalization;
using System;

namespace Gamegaard.RuntimeDebug
{
    public class TupleParser2<T1, T2> : ITypeParser<(T1, T2)>
    {
        public (T1, T2) Parse(string value)
        {
            string[] parts = value.Split(',');

            if (parts.Length != 2)
                throw new FormatException($"Input '{value}' cannot be parsed into a tuple of 2 elements.");

            T1 item1 = (T1)Convert.ChangeType(parts[0].Trim(), typeof(T1), CultureInfo.InvariantCulture);
            T2 item2 = (T2)Convert.ChangeType(parts[1].Trim(), typeof(T2), CultureInfo.InvariantCulture);

            return (item1, item2);
        }
    }
}