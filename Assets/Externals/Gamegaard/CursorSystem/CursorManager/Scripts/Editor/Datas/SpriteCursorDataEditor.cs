using UnityEditor;
using UnityEngine;

namespace Gamegaard.CursorSystem.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SpriteCursorData))]
    public class SpriteCursorDataEditor : GenericCursorDataEditor<Sprite> { }
}
