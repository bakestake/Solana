using UnityEditor;
using UnityEngine;

namespace Gamegaard.CursorSystem.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(TextureCursorData))]
    public class TextureCursorDataEditor : GenericCursorDataEditor<Texture2D> { }
}
