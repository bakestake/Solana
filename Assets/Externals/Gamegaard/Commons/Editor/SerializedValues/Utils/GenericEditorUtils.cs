using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Gamegaard.CustomValues.Editor
{
    public static class GenericEditorUtils
    {
        public static List<Type> DefaultSupportedValues = new List<Type>
        {
            typeof(int),
            typeof(bool),
            typeof(float),
            typeof(string),
            typeof(char),
            typeof(Color),
            typeof(LayerMask),
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector2Int),
            typeof(Vector3Int),
            typeof(Vector4),
            typeof(Quaternion),
            typeof(Bounds),
            typeof(BoundsInt),
            typeof(Rect),
            typeof(RectInt),
            typeof(Enum),
            typeof(AnimationCurve),
            typeof(UnityEngine.Object)
        };

        private static readonly Dictionary<Type, Func<Rect, object, object>> DrawValueActions = new Dictionary<Type, Func<Rect, object, object>>
        {
            { typeof(int), (rect, value) => EditorGUI.IntField(rect, (int)value) },
            { typeof(bool), (rect, value) => EditorGUI.Toggle(rect, (bool)value) },
            { typeof(float), (rect, value) => EditorGUI.FloatField(rect, (float)value) },
            { typeof(string), (rect, value) => EditorGUI.TextField(rect, (string)value) },
            { typeof(Color), (rect, value) => EditorGUI.ColorField(rect, (Color)value) },
            { typeof(LayerMask), (rect, value) => EditorGUI.LayerField(rect, (int)value) },
            { typeof(Vector2), (rect, value) => EditorGUI.Vector2Field(rect, GUIContent.none, (Vector2)value) },
            { typeof(Vector3), (rect, value) => EditorGUI.Vector3Field(rect, GUIContent.none, (Vector3)value) },
            { typeof(Vector4), (rect, value) => EditorGUI.Vector4Field(rect, GUIContent.none, (Vector4)value) },
            { typeof(Rect), (rect, value) => EditorGUI.RectField(rect, (Rect)value) },
            { typeof(char), (rect, value) => EditorGUI.TextField(rect, ((char)value).ToString()).FirstOrDefault() },
            { typeof(AnimationCurve), (rect, value) => EditorGUI.CurveField(rect, (AnimationCurve)value) },
            { typeof(Bounds), (rect, value) => EditorGUI.BoundsField(rect, (Bounds)value) },
            { typeof(Quaternion), (rect, value) =>
                {
                    Quaternion quat = (Quaternion)value;
                    Vector4 quatVector = new Vector4(quat.x, quat.y, quat.z, quat.w);
                    return EditorGUI.Vector4Field(rect, GUIContent.none, quatVector);
                }
            },
            { typeof(Vector2Int), (rect, value) => EditorGUI.Vector2IntField(rect, GUIContent.none, (Vector2Int)value) },
            { typeof(Vector3Int), (rect, value) => EditorGUI.Vector3IntField(rect, GUIContent.none, (Vector3Int)value) },
            { typeof(RectInt), (rect, value) => EditorGUI.RectIntField(rect, (RectInt)value) },
            { typeof(BoundsInt), (rect, value) => EditorGUI.BoundsIntField(rect, (BoundsInt)value) },
            { typeof(UnityEngine.Object), (rect, value) => EditorGUI.ObjectField(rect, (UnityEngine.Object)value, typeof(UnityEngine.Object), true) }
        };

        private static readonly Dictionary<Type, Func<Rect, GUIContent, SerializedProperty, object>> DrawManagedValueActions = new Dictionary<Type, Func<Rect, GUIContent, SerializedProperty, object>>
        {
            { typeof(int), (rect, guiContent, prop) => EditorGUI.IntField(rect, guiContent, (int)prop.managedReferenceValue) },
            { typeof(bool), (rect, guiContent, prop) => EditorGUI.Toggle(rect, guiContent, (bool)prop.managedReferenceValue) },
            { typeof(float), (rect, guiContent, prop) => EditorGUI.FloatField(rect, guiContent, (float)prop.managedReferenceValue) },
            { typeof(string), (rect, guiContent, prop) => EditorGUI.TextField(rect, guiContent, (string)prop.managedReferenceValue) },
            { typeof(Color), (rect, guiContent, prop) => EditorGUI.ColorField(rect, guiContent, (Color)prop.managedReferenceValue) },
            { typeof(LayerMask), (rect, guiContent, prop) => EditorGUI.LayerField(rect, guiContent, (int)prop.managedReferenceValue) },
            { typeof(Vector2), (rect, guiContent, prop) => EditorGUI.Vector2Field(rect, guiContent,  (Vector2)prop.managedReferenceValue) },
            { typeof(Vector3), (rect, guiContent, prop) => EditorGUI.Vector3Field(rect, guiContent,  (Vector3)prop.managedReferenceValue) },
            { typeof(Vector4), (rect, guiContent, prop) => EditorGUI.Vector4Field(rect, guiContent, (Vector4)prop.managedReferenceValue) },
            { typeof(Rect), (rect, guiContent, prop) => EditorGUI.RectField(rect, guiContent, (Rect)prop.managedReferenceValue) },
            { typeof(char), (rect, guiContent, prop) => EditorGUI.TextField(rect, guiContent, ((char)prop.managedReferenceValue).ToString()).FirstOrDefault() },
            { typeof(AnimationCurve), (rect, guiContent, prop) => EditorGUI.CurveField(rect, guiContent, (AnimationCurve)prop.managedReferenceValue) },
            { typeof(Bounds), (rect, guiContent, prop) => EditorGUI.BoundsField(rect, guiContent, (Bounds)prop.managedReferenceValue) },
            { typeof(Quaternion), (rect, guiContent, prop) =>
                {
                    Quaternion quat = (Quaternion)prop.managedReferenceValue;
                    Vector4 quatVector = new Vector4(quat.x, quat.y, quat.z, quat.w);
                    return EditorGUI.Vector4Field(rect, guiContent, quatVector);
                }
            },
            { typeof(Vector2Int), (rect, guiContent, prop) => EditorGUI.Vector2IntField(rect, guiContent, (Vector2Int)prop.managedReferenceValue) },
            { typeof(Vector3Int), (rect, guiContent, prop) => EditorGUI.Vector3IntField(rect, guiContent, (Vector3Int)prop.managedReferenceValue) },
            { typeof(RectInt), (rect, guiContent, prop) => EditorGUI.RectIntField(rect, guiContent, (RectInt)prop.managedReferenceValue) },
            { typeof(BoundsInt), (rect, guiContent, prop) => EditorGUI.BoundsIntField(rect, guiContent, (BoundsInt)prop.managedReferenceValue) },
            { typeof(UnityEngine.Object), (rect, guiContent, prop) => EditorGUI.ObjectField(rect, guiContent, (UnityEngine.Object)prop.managedReferenceValue, typeof(UnityEngine.Object), true) },
        };

        private static readonly Dictionary<SerializedPropertyType, Func<Rect, SerializedProperty, object>> DrawPropertyActions = new Dictionary<SerializedPropertyType, Func<Rect, SerializedProperty, object>>
        {
            { SerializedPropertyType.Integer, (rect, prop) => EditorGUI.IntField(rect, prop.intValue) },
            { SerializedPropertyType.Boolean, (rect, prop) => EditorGUI.Toggle(rect, prop.boolValue) },
            { SerializedPropertyType.Float, (rect, prop) => EditorGUI.FloatField(rect, prop.floatValue) },
            { SerializedPropertyType.String, (rect, prop) => EditorGUI.TextField(rect, prop.stringValue) },
            { SerializedPropertyType.Color, (rect, prop) => EditorGUI.ColorField(rect, prop.colorValue) },
            { SerializedPropertyType.ObjectReference, (rect, prop) => EditorGUI.ObjectField(rect, prop.objectReferenceValue, typeof(UnityEngine.Object), true) },
            { SerializedPropertyType.LayerMask, (rect, prop) => EditorGUI.LayerField(rect, prop.intValue) },
            { SerializedPropertyType.Enum, (rect, prop) => EditorGUI.EnumPopup(rect, (Enum)Enum.ToObject(GetEnumType(prop), prop.enumValueIndex)) },
            { SerializedPropertyType.Vector2, (rect, prop) => EditorGUI.Vector2Field(rect, GUIContent.none, prop.vector2Value) },
            { SerializedPropertyType.Vector3, (rect, prop) => EditorGUI.Vector3Field(rect, GUIContent.none, prop.vector3Value) },
            { SerializedPropertyType.Vector4, (rect, prop) => EditorGUI.Vector4Field(rect, GUIContent.none, prop.vector4Value) },
            { SerializedPropertyType.Rect, (rect, prop) => EditorGUI.RectField(rect, prop.rectValue) },
            { SerializedPropertyType.Character, (rect, prop) =>
                {
                    string text = EditorGUI.TextField(rect, prop.intValue > 0 ? ((char)prop.intValue).ToString() : "");

                    if (!string.IsNullOrEmpty(text))
                    {
                        prop.intValue = text[0];
                    }

                    return prop.intValue;
                }
            },
            { SerializedPropertyType.AnimationCurve, (rect, prop) => EditorGUI.CurveField(rect, prop.animationCurveValue) },
            { SerializedPropertyType.Bounds, (rect, prop) => EditorGUI.BoundsField(rect, prop.boundsValue) },
            { SerializedPropertyType.Quaternion, (rect, prop) =>
                {
                    Vector4 quatVector = new Vector4(prop.quaternionValue.x, prop.quaternionValue.y, prop.quaternionValue.z, prop.quaternionValue.w);
                    return EditorGUI.Vector4Field(rect, GUIContent.none, quatVector);
                }
            },
            { SerializedPropertyType.ExposedReference, (rect, prop) =>
                {
                    prop.exposedReferenceValue = EditorGUI.ObjectField(rect, prop.exposedReferenceValue, typeof(UnityEngine.Object), true);
                    return prop.exposedReferenceValue;
                }
            },
            { SerializedPropertyType.Vector2Int, (rect, prop) => EditorGUI.Vector2IntField(rect, GUIContent.none, prop.vector2IntValue) },
            { SerializedPropertyType.Vector3Int, (rect, prop) => EditorGUI.Vector3IntField(rect, GUIContent.none, prop.vector3IntValue) },
            { SerializedPropertyType.RectInt, (rect, prop) => EditorGUI.RectIntField(rect, prop.rectIntValue) },
            { SerializedPropertyType.BoundsInt, (rect, prop) => EditorGUI.BoundsIntField(rect, prop.boundsIntValue) },
            { SerializedPropertyType.ManagedReference, (rect, prop) =>
                {
                    EditorGUI.PropertyField(rect, prop, true);
                    return prop.managedReferenceValue;
                }
            }
        };

        private static readonly Dictionary<SerializedPropertyType, Func<SerializedProperty, object>> GetValueActions = new Dictionary<SerializedPropertyType, Func<SerializedProperty, object>>
        {
            { SerializedPropertyType.Integer, prop => prop.intValue },
            { SerializedPropertyType.Boolean, prop => prop.boolValue },
            { SerializedPropertyType.Float, prop => prop.floatValue },
            { SerializedPropertyType.String, prop => prop.stringValue },
            { SerializedPropertyType.Color, prop => prop.colorValue },
            { SerializedPropertyType.ObjectReference, prop => prop.objectReferenceValue },
            { SerializedPropertyType.LayerMask, prop => prop.intValue },
            { SerializedPropertyType.Enum, prop => prop.enumValueIndex },
            { SerializedPropertyType.Vector2, prop => prop.vector2Value },
            { SerializedPropertyType.Vector3, prop => prop.vector3Value },
            { SerializedPropertyType.Vector4, prop => prop.vector4Value },
            { SerializedPropertyType.Rect, prop => prop.rectValue },
            { SerializedPropertyType.Character, prop => (char)prop.intValue },
            { SerializedPropertyType.AnimationCurve, prop => prop.animationCurveValue },
            { SerializedPropertyType.Bounds, prop => prop.boundsValue },
            { SerializedPropertyType.Quaternion, prop => prop.quaternionValue },
            { SerializedPropertyType.ExposedReference, prop => prop.exposedReferenceValue },
            { SerializedPropertyType.Vector2Int, prop => prop.vector2IntValue },
            { SerializedPropertyType.Vector3Int, prop => prop.vector3IntValue },
            { SerializedPropertyType.RectInt, prop => prop.rectIntValue },
            { SerializedPropertyType.BoundsInt, prop => prop.boundsIntValue },
            { SerializedPropertyType.ManagedReference, prop => prop.managedReferenceValue }
        };

        private static readonly Dictionary<SerializedPropertyType, Action<SerializedProperty, object>> SetValueActions = new Dictionary<SerializedPropertyType, Action<SerializedProperty, object>>
        {
            { SerializedPropertyType.Integer, (prop, value) => prop.intValue = (int)value },
            { SerializedPropertyType.Boolean, (prop, value) => prop.boolValue = (bool)value },
            { SerializedPropertyType.Float, (prop, value) => prop.floatValue = (float)value },
            { SerializedPropertyType.String, (prop, value) => prop.stringValue = (string)value },
            { SerializedPropertyType.Color, (prop, value) => prop.colorValue = (Color)value },
            { SerializedPropertyType.ObjectReference, (prop, value) => prop.objectReferenceValue = (UnityEngine.Object)value },
            { SerializedPropertyType.LayerMask, (prop, value) => prop.intValue = (int)value },
            { SerializedPropertyType.Enum, (prop, value) => prop.enumValueIndex = (int)value },
            { SerializedPropertyType.Vector2, (prop, value) => prop.vector2Value = (Vector2)value },
            { SerializedPropertyType.Vector3, (prop, value) => prop.vector3Value = (Vector3)value },
            { SerializedPropertyType.Vector4, (prop, value) => prop.vector4Value = (Vector4)value },
            { SerializedPropertyType.Rect, (prop, value) => prop.rectValue = (Rect)value },
            { SerializedPropertyType.Character, (prop, value) => prop.intValue = (int)(char)value },
            { SerializedPropertyType.AnimationCurve, (prop, value) => prop.animationCurveValue = (AnimationCurve)value },
            { SerializedPropertyType.Bounds, (prop, value) => prop.boundsValue = (Bounds)value },
            { SerializedPropertyType.Quaternion, (prop, value) => prop.quaternionValue = (Quaternion)value },
            { SerializedPropertyType.ExposedReference, (prop, value) => prop.exposedReferenceValue = (UnityEngine.Object)value },
            { SerializedPropertyType.Vector2Int, (prop, value) => prop.vector2IntValue = (Vector2Int)value },
            { SerializedPropertyType.Vector3Int, (prop, value) => prop.vector3IntValue = (Vector3Int)value },
            { SerializedPropertyType.RectInt, (prop, value) => prop.rectIntValue = (RectInt)value },
            { SerializedPropertyType.BoundsInt, (prop, value) => prop.boundsIntValue = (BoundsInt)value },
            { SerializedPropertyType.ManagedReference, (prop, value) => prop.managedReferenceValue = value }
        };

        public static object GetGenericPropertyValue(SerializedProperty property)
        {
            if (GetValueActions.TryGetValue(property.propertyType, out var getAction))
            {
                return getAction(property);
            }
            else
            {
                Debug.LogWarning($"{property.propertyType} type is not supported.");
                return null;
            }
        }

        public static void SetGenericPropertyValue(SerializedProperty property, object value)
        {
            if (SetValueActions.TryGetValue(property.propertyType, out var setAction))
            {
                setAction(property, value);
            }
            else
            {
                Debug.LogWarning($"{property.propertyType} type is not supported.");
            }
        }

        public static object DrawPropertyField(Rect rect, object value)
        {
            if (value == null)
            {
                EditorGUI.LabelField(rect, "Null");
                return null;
            }

            Type valueType = value.GetType();

            if (DrawValueActions.TryGetValue(valueType, out var drawAction))
            {
                return drawAction(rect, value);
            }
            else if (valueType.IsEnum)
            {
                return EditorGUI.EnumPopup(rect, (Enum)value);
            }
            else if (typeof(UnityEngine.Object).IsAssignableFrom(valueType))
            {
                return EditorGUI.ObjectField(rect, (UnityEngine.Object)value, valueType, true);
            }
            else
            {
                EditorGUI.LabelField(rect, value.ToString());
                return value;
            }
        }

        public static object DrawManagedPropertyField(Rect rect, SerializedProperty property, GUIContent guiContent, Type fieldType)
        {
            if (DrawManagedValueActions.TryGetValue(fieldType, out var drawAction))
            {
                rect.height = EditorGUIUtility.singleLineHeight;
                return drawAction(rect, guiContent, property);
            }

            return EditorGUI.PropertyField(rect, property, true);
        }

        public static object DrawPropertyField(Rect rect, SerializedProperty property)
        {
            if (DrawPropertyActions.TryGetValue(property.propertyType, out var drawAction))
            {
                return drawAction(rect, property);
            }

            EditorGUI.LabelField(rect, $"[Unsupported] {property.propertyType}");
            return null;
        }

        private static Type GetEnumType(SerializedProperty property)
        {
            Type parentType = property.serializedObject.targetObject.GetType();
            FieldInfo field = parentType.GetField(property.propertyPath, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            return field?.FieldType ?? typeof(Enum);
        }

        #region KeyGenerator
        public static object GenerateUniqueKey(Type keyType, SerializedProperty keyValuePairsProp)
        {
            if (keyType == null) return null;

            HashSet<object> existingKeys = new HashSet<object>();

            for (int i = 0; i < keyValuePairsProp.arraySize; i++)
            {
                SerializedProperty keyProp = keyValuePairsProp.GetArrayElementAtIndex(i).FindPropertyRelative("key");
                existingKeys.Add(GetKeyValue(keyProp, keyType));
            }

            if (keyType == typeof(string))
            {
                return GenerateUniqueString(existingKeys);
            }
            if (keyType == typeof(int))
            {
                return GenerateUniqueInt(existingKeys);
            }
            if (keyType == typeof(float))
            {
                return GenerateUniqueFloat(existingKeys);
            }
            if (keyType.IsEnum)
            {
                return GenerateUniqueEnum(keyType, existingKeys);
            }

            return null;
        }

        private static string GenerateUniqueString(HashSet<object> existingKeys)
        {
            int counter = 0;
            string uniqueName;
            do
            {
                uniqueName = $"Element {counter}";
                counter++;
            } while (existingKeys.Contains(uniqueName));

            return uniqueName;
        }

        private static int GenerateUniqueInt(HashSet<object> existingKeys)
        {
            int value = 1;
            while (existingKeys.Contains(value))
            {
                value++;
            }
            return value;
        }

        private static float GenerateUniqueFloat(HashSet<object> existingKeys)
        {
            float value = 1f;
            while (existingKeys.Contains(value))
            {
                value++;
            }
            return value;
        }

        private static object GenerateUniqueEnum(Type enumType, HashSet<object> existingKeys)
        {
            Array enumValues = Enum.GetValues(enumType);
            foreach (object enumValue in enumValues)
            {
                if (!existingKeys.Contains(enumValue))
                {
                    return enumValue;
                }
            }
            return enumValues.GetValue(0);
        }

        private static object GetKeyValue(SerializedProperty keyProp, Type keyType)
        {
            if (keyType == null) return null;

            if (keyType == typeof(string)) return keyProp.stringValue;
            if (keyType == typeof(int)) return keyProp.intValue;
            if (keyType == typeof(float)) return keyProp.floatValue;
            if (keyType.IsEnum) return Enum.ToObject(keyType, keyProp.intValue);

            return null;
        }
        #endregion
    }
}