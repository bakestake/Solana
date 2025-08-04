using System.Globalization;
using UnityEngine;

namespace Gamegaard.RuntimeDebug
{
    public class Vector2Parser : ITypeParser<Vector2>
    {
        public Vector2 Parse(string value)
        {
            string[] parts = value.Split(',');
            return new Vector2(
                float.Parse(parts[0], CultureInfo.InvariantCulture),
                float.Parse(parts[1], CultureInfo.InvariantCulture));
        }
    }
}