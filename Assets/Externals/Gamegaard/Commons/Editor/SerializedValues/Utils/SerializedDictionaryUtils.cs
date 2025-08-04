using System;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Gamegaard.CustomValues.Editor
{
    public static class SerializedDictionaryUtils
    {
        public static bool TryParse(string input, out Hash128 hash)
        {
            if (input.Length == 32 && ulong.TryParse(input, System.Globalization.NumberStyles.HexNumber, null, out ulong low64) &&
                ulong.TryParse(input.Substring(16), System.Globalization.NumberStyles.HexNumber, null, out ulong high64))
            {
                hash = new Hash128(low64, high64);
                return true;
            }
            hash = default;
            return false;
        }

        public static string GetValueAsString(this SerializedProperty keyProp)
        {
            switch (keyProp.propertyType)
            {
                case SerializedPropertyType.String:
                    return keyProp.stringValue;
                case SerializedPropertyType.Integer:
                    return keyProp.intValue.ToString();
                case SerializedPropertyType.Float:
                    return keyProp.floatValue.ToString();
                case SerializedPropertyType.Boolean:
                    return keyProp.boolValue.ToString();
                case SerializedPropertyType.Color:
                    return keyProp.colorValue.ToString();
                case SerializedPropertyType.LayerMask:
                    return keyProp.intValue.ToString();
                case SerializedPropertyType.Enum:
                    return keyProp.enumNames[keyProp.enumValueIndex];
                case SerializedPropertyType.Vector2:
                    return keyProp.vector2Value.ToString();
                case SerializedPropertyType.Vector3:
                    return keyProp.vector3Value.ToString();
                case SerializedPropertyType.Vector4:
                    return keyProp.vector4Value.ToString();
                case SerializedPropertyType.Rect:
                    return keyProp.rectValue.ToString();
                case SerializedPropertyType.ArraySize:
                    return keyProp.arraySize.ToString();
                case SerializedPropertyType.Character:
                    return ((char)keyProp.intValue).ToString();
                case SerializedPropertyType.AnimationCurve:
                    return keyProp.animationCurveValue.ToString();
                case SerializedPropertyType.Bounds:
                    return keyProp.boundsValue.ToString();
#if UNITY_2022_1_OR_NEWER
                case SerializedPropertyType.Gradient:
                    return keyProp.gradientValue.ToString();
#endif
                case SerializedPropertyType.Quaternion:
                    return keyProp.quaternionValue.ToString();
                case SerializedPropertyType.ExposedReference:
                    return keyProp.objectReferenceValue != null ? keyProp.objectReferenceValue.ToString() : null;
                case SerializedPropertyType.FixedBufferSize:
                    return keyProp.intValue.ToString();
                case SerializedPropertyType.Vector2Int:
                    return keyProp.vector2IntValue.ToString();
                case SerializedPropertyType.Vector3Int:
                    return keyProp.vector3IntValue.ToString();
                case SerializedPropertyType.RectInt:
                    return keyProp.rectIntValue.ToString();
                case SerializedPropertyType.BoundsInt:
                    return keyProp.boundsIntValue.ToString();
#if UNITY_2022_1_OR_NEWER
                case SerializedPropertyType.ManagedReference:
                    return keyProp.managedReferenceId.ToString();
#endif
#if UNITY_2023_1_OR_NEWER
                case SerializedPropertyType.Hash128:
                    return keyProp.hash128Value.ToString();
#endif
                case SerializedPropertyType.Generic:
                default:
                    return null;
            }
        }

        public static bool IsKeyPresent(string keyValue, SerializedProperty keyValuePairsListProp)
        {
            for (int i = 0; i < keyValuePairsListProp.arraySize; i++)
            {
                SerializedProperty element = keyValuePairsListProp.GetArrayElementAtIndex(i);
                SerializedProperty currentKeyProp = element.FindPropertyRelative("key");

                if (currentKeyProp.GetValueAsString() == keyValue)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsDuplicateKey(SerializedProperty keyValuePairsProp, SerializedProperty keyProp, int currentIndex)
        {
            string currentKeyValue = keyProp.GetValueAsString();
            for (int i = 0; i < keyValuePairsProp.arraySize; i++)
            {
                if (i == currentIndex) continue;

                SerializedProperty element = keyValuePairsProp.GetArrayElementAtIndex(i);
                SerializedProperty currentKeyProp = element.FindPropertyRelative("key");
                string keyValue = currentKeyProp.GetValueAsString();

                if (!string.IsNullOrEmpty(keyValue) && keyValue == currentKeyValue)
                {
                    return true;
                }
            }
            return false;
        }

        public static T GetNextKeyValue<T>(this SerializedProperty keyProp, SerializedProperty keyValuePairsListProp)
        {
            int index = 1;

            switch (keyProp.propertyType)
            {
                case SerializedPropertyType.String:
                    string baseValue = keyProp.stringValue;
                    string newValue = baseValue;

                    if (IsKeyPresent(newValue, keyValuePairsListProp))
                    {
                        string pattern = @"_(\d+)$";
                        Match match = Regex.Match(baseValue, pattern);

                        if (match.Success)
                        {
                            int existingIndex = int.Parse(match.Groups[1].Value);
                            baseValue = baseValue.Substring(0, match.Index);
                            index = existingIndex + 1;
                        }

                        while (index < int.MaxValue)
                        {
                            newValue = $"{baseValue}_{index}";

                            if (!IsKeyPresent(newValue, keyValuePairsListProp))
                            {
                                return (T)(object)newValue;
                            }
                            index++;
                        }
                    }
                    return (T)(object)newValue;

                case SerializedPropertyType.Integer:
                    int intValue = keyProp.intValue;
                    while (intValue < int.MaxValue)
                    {
                        intValue++;
                        if (!IsKeyPresent(intValue.ToString(), keyValuePairsListProp))
                        {
                            return (T)(object)intValue;
                        }
                    }
                    break;

                case SerializedPropertyType.Float:
                    float floatValue = keyProp.floatValue;
                    while (floatValue < float.MaxValue)
                    {
                        floatValue++;
                        if (!IsKeyPresent(floatValue.ToString(), keyValuePairsListProp))
                        {
                            return (T)(object)floatValue;
                        }
                    }
                    break;

                case SerializedPropertyType.Boolean:
                    bool currentBool = keyProp.boolValue;
                    bool newBoolValue = !currentBool;
                    if (!IsKeyPresent(newBoolValue.ToString(), keyValuePairsListProp))
                    {
                        return (T)(object)newBoolValue;
                    }
                    break;

                case SerializedPropertyType.Enum:
                    int enumIndex = keyProp.enumValueIndex + 1;
                    while (enumIndex < keyProp.enumNames.Length)
                    {
                        string enumName = keyProp.enumNames[enumIndex % keyProp.enumNames.Length];
                        if (!IsKeyPresent(enumName, keyValuePairsListProp))
                        {
                            return (T)(object)enumName;
                        }
                        enumIndex++;
                    }
                    break;

                case SerializedPropertyType.Character:
                    int currentCharValue = keyProp.intValue;
                    int nextCharValue = currentCharValue + 1;

                    while (nextCharValue <= char.MaxValue)
                    {
                        if (!IsKeyPresent(((char)nextCharValue).ToString(), keyValuePairsListProp))
                        {
                            return (T)(object)nextCharValue;
                        }
                        nextCharValue++;
                    }
                    break;
            }

            return default;
        }

        public static void AdjustDictionaryListSize(this SerializedProperty property, int newSize)
        {
            SerializedProperty keyValuePairsProp = property.FindPropertyRelative("keyValuePairs");

            while (keyValuePairsProp.arraySize < newSize)
            {
                keyValuePairsProp.InsertArrayElementAtIndex(keyValuePairsProp.arraySize);
            }

            while (keyValuePairsProp.arraySize > newSize)
            {
                keyValuePairsProp.DeleteArrayElementAtIndex(keyValuePairsProp.arraySize - 1);
            }

            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }

        public static void AssignDefaultValues(this SerializedProperty keyProp, SerializedProperty keyValuePairsListProp)
        {
            switch (keyProp.propertyType)
            {
                case SerializedPropertyType.String:
                    string newStringKeyValue = keyProp.GetNextKeyValue<string>(keyValuePairsListProp);
                    keyProp.stringValue = newStringKeyValue;
                    break;

                case SerializedPropertyType.Integer:
                    int newIntKeyValue = keyProp.GetNextKeyValue<int>(keyValuePairsListProp);
                    keyProp.intValue = newIntKeyValue;
                    break;

                case SerializedPropertyType.Float:
                    float newFloatKeyValue = keyProp.GetNextKeyValue<float>(keyValuePairsListProp);
                    keyProp.floatValue = newFloatKeyValue;
                    break;

                case SerializedPropertyType.Boolean:
                    bool newBoolKeyValue = keyProp.GetNextKeyValue<bool>(keyValuePairsListProp);
                    keyProp.boolValue = newBoolKeyValue;
                    break;

                case SerializedPropertyType.Character:
                    int newCharKeyValue = keyProp.GetNextKeyValue<int>(keyValuePairsListProp);
                    keyProp.intValue = newCharKeyValue;
                    break;

                case SerializedPropertyType.Enum:
                    string newEnumKeyValue = keyProp.GetNextKeyValue<string>(keyValuePairsListProp);
                    int newEnumIndex = Array.IndexOf(keyProp.enumNames, newEnumKeyValue);

                    if (newEnumIndex >= 0 && newEnumIndex < keyProp.enumNames.Length)
                    {
                        keyProp.enumValueIndex = newEnumIndex;
                    }
                    break;
            }
        }

        public static T GetValueFromSerializedProperty<T>(this SerializedProperty property, FieldInfo fieldInfo) where T : class
        {
            object container = fieldInfo.GetValue(property.serializedObject.targetObject);

            if (property.propertyPath.Contains("Array"))
            {
                string[] pathParts = property.propertyPath.Split('.');
                foreach (string part in pathParts)
                {
                    if (part.StartsWith("data["))
                    {
                        int index = int.Parse(part.Replace("data[", "").Replace("]", ""));
                        if (container is System.Collections.IList list && index < list.Count)
                        {
                            container = list[index];
                        }
                    }
                }
            }

            return container as T;
        }

    }
}