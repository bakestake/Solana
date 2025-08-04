using System.Globalization;
using UnityEngine;

namespace Gamegaard.RuntimeDebug
{
    public class Vector3Parser : ITypeParser<Vector3>
    {
        public Vector3 Parse(string value)
        {
            string[] parts = value.Split(',');
            return new Vector3(
                float.Parse(parts[0], CultureInfo.InvariantCulture),
                float.Parse(parts[1], CultureInfo.InvariantCulture),
                float.Parse(parts[2], CultureInfo.InvariantCulture));
        }
    }
}