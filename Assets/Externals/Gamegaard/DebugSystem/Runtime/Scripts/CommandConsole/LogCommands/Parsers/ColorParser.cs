using UnityEngine;

namespace Gamegaard.RuntimeDebug
{
    public class ColorParser : ITypeParser<Color>
    {
        public Color Parse(string value)
        {
            return ColorUtility.TryParseHtmlString(value.StartsWith("#") ? value : $"#{value}", out Color color) ? color : Color.white;
        }
    }
}