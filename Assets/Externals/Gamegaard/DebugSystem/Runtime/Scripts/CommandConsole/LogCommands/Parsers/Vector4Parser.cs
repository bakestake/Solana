using System.Globalization;
using UnityEngine;

namespace Gamegaard.RuntimeDebug
{
    public class Vector4Parser : ITypeParser<Vector4>
    {
        public Vector4 Parse(string value)
        {
            string[] parts = value.Split(',');
            return new Vector4(
                float.Parse(parts[0], CultureInfo.InvariantCulture),
                float.Parse(parts[1], CultureInfo.InvariantCulture),
                float.Parse(parts[2], CultureInfo.InvariantCulture),
                float.Parse(parts[3], CultureInfo.InvariantCulture));
        }
    }
}