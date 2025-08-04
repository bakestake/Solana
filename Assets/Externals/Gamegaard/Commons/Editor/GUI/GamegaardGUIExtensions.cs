using System;
using UnityEditor;

namespace Gamegaard.Commons.Editor.Utils
{
    public static class GamegaardGUIExtensions
    {
        public static Type GetObjectType(this SerializedProperty property)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                return property.objectReferenceValue?.GetType();
            }

            return null;
        }

        public static bool ValidateType(this SerializedProperty property, Type type)
        {
            Type objectType = GetObjectType(property);
            return type.IsAssignableFrom(objectType);
        }
    }
}
