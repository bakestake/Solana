using System.Globalization;
using UnityEngine;

namespace Gamegaard.RuntimeDebug
{
    public class QuaternionParser : ITypeParser<Quaternion>
    {
        public Quaternion Parse(string value)
        {
            string[] parts = value.Split(',');
            return new Quaternion(
                float.Parse(parts[0], CultureInfo.InvariantCulture),
                float.Parse(parts[1], CultureInfo.InvariantCulture),
                float.Parse(parts[2], CultureInfo.InvariantCulture),
                float.Parse(parts[3], CultureInfo.InvariantCulture));
        }
    }
}