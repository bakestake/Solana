using Gamegaard.Commons.Editor;
using Gamegaard.CustomValues;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

namespace Gamegaard.SerializableAttributes.Editor
{
    public static class SerializableEditorElements
    {
        #region Attributes

        public static readonly Dictionary<Type, BasicSerializableValuesTypes> TypeToValueType = new Dictionary<Type, BasicSerializableValuesTypes>
        {
            { typeof(IntAttributeValue), BasicSerializableValuesTypes.Int },
            { typeof(FloatAttributeValue), BasicSerializableValuesTypes.Float },
            { typeof(StringAttributeValue), BasicSerializableValuesTypes.String },
            { typeof(BoolAttributeValue), BasicSerializableValuesTypes.Bool },
            { typeof(CharAttributeValue), BasicSerializableValuesTypes.Char },
            { typeof(GameObjectAttributeValue), BasicSerializableValuesTypes.GameObject },
            { typeof(UnityObjectAttributeValue), BasicSerializableValuesTypes.UnityObject },
            { typeof(ScriptableObjectAttributeValue), BasicSerializableValuesTypes.ScriptableObject },
            { typeof(EnumAttributeValue), BasicSerializableValuesTypes.Enum },
            { typeof(CustomAttributeValue), BasicSerializableValuesTypes.CustomObject }
        };

        public static readonly Dictionary<BasicSerializableValuesTypes, Func<IAttributeValue>> DefaultValues = new Dictionary<BasicSerializableValuesTypes, Func<IAttributeValue>>
        {
            { BasicSerializableValuesTypes.Int, () => new IntAttributeValue() },
            { BasicSerializableValuesTypes.Float, () => new FloatAttributeValue() },
            { BasicSerializableValuesTypes.String, () => new StringAttributeValue() },
            { BasicSerializableValuesTypes.Bool, () => new BoolAttributeValue() },
            { BasicSerializableValuesTypes.Char, () => new CharAttributeValue() },
            { BasicSerializableValuesTypes.GameObject, () => new GameObjectAttributeValue() },
            { BasicSerializableValuesTypes.UnityObject, () => new UnityObjectAttributeValue() },
            { BasicSerializableValuesTypes.ScriptableObject, () => new ScriptableObjectAttributeValue() },
            { BasicSerializableValuesTypes.Enum, () => new EnumAttributeValue() },
            { BasicSerializableValuesTypes.CustomObject, () => new CustomAttributeValue() }
        };

        private static BasicSerializableValuesTypes GetAttributeType(object value)
        {
            if (value == null) return BasicSerializableValuesTypes.String;
            Type type = value.GetType();
            return TypeToValueType.TryGetValue(type, out var result) ? result : BasicSerializableValuesTypes.String;
        }

        private static IAttributeValue GetDefaultValue(BasicSerializableValuesTypes type)
        {
            return DefaultValues.TryGetValue(type, out var factory) ? factory() : null;
        }

        public static IAttributeValue DrawAttributeValueField(IAttributeValue value, Rect lineRect, Rect fieldRect, SerializedProperty property = null)
        {
            if (value == null) return null;

            return value switch
            {
                IntAttributeValue intValue => new IntAttributeValue { Value = EditorGUI.IntField(fieldRect, intValue.Value) },
                FloatAttributeValue floatValue => new FloatAttributeValue { Value = EditorGUI.FloatField(fieldRect, floatValue.Value) },
                StringAttributeValue stringValue => new StringAttributeValue { Value = EditorGUI.TextField(fieldRect, stringValue.Value) },
                BoolAttributeValue boolValue => new BoolAttributeValue { Value = EditorGUI.Toggle(fieldRect, boolValue.Value) },
                CharAttributeValue charValue => new CharAttributeValue { Value = EditorGUI.TextField(fieldRect, charValue.Value.ToString()).FirstOrDefault() },
                GameObjectAttributeValue gameObjectValue => new GameObjectAttributeValue { Value = (GameObject)EditorGUI.ObjectField(fieldRect, gameObjectValue.Value, typeof(GameObject), true) },
                UnityObjectAttributeValue unityObjectValue => new UnityObjectAttributeValue { Value = EditorGUI.ObjectField(fieldRect, unityObjectValue.Value, typeof(UnityEngine.Object), true) },
                ScriptableObjectAttributeValue scriptableObjectValue => new ScriptableObjectAttributeValue { Value = (ScriptableObject)EditorGUI.ObjectField(fieldRect, scriptableObjectValue.Value, typeof(ScriptableObject), true) },

                EnumAttributeValue enumValue => property != null
                    ? DrawEnumField(enumValue, lineRect, fieldRect, property)
                    : DrawEnumField(enumValue, fieldRect),

                CustomAttributeValue customValue => property != null
                    ? DrawCustomField(customValue, lineRect, fieldRect, property)
                    : DrawCustomField(customValue, lineRect, fieldRect),

                _ => value
            };
        }

        public static ReorderableList CreateAttributesList(SerializedProperty property, GUIContent label, System.Action onChange = null)
        {
            SerializedProperty keyValuePairs;

            if (property.name == "keyValuePairs")
            {
                keyValuePairs = property;
            }
            else
            {
                keyValuePairs = property.FindPropertyRelative("keyValuePairs");
                if (keyValuePairs == null || !keyValuePairs.isArray)
                {
                    Debug.LogError($"[CreateAttributesList] Erro: 'keyValuePairs' não encontrado ou não é um array em '{property.propertyPath}'");
                    return null;
                }
            }

            ReorderableList reorderableList = new ReorderableList(keyValuePairs.serializedObject, keyValuePairs)
            {
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, label),

                elementHeightCallback = index => GetElementHeight(keyValuePairs, index),

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    SerializedProperty element = keyValuePairs.GetArrayElementAtIndex(index);
                    if (element == null) return;

                    SerializedProperty keyProp = element.FindPropertyRelative("key");
                    SerializedProperty valueProp = element.FindPropertyRelative("value");

                    rect.y += 2;

                    EditorGUI.BeginChangeCheck();

                    DrawAttributeField(rect, keyProp, valueProp);

                    if (EditorGUI.EndChangeCheck())
                    {
                        keyValuePairs.serializedObject.ApplyModifiedProperties();
                    }
                },

                onAddDropdownCallback = (buttonRect, list) =>
                {
                    ShowAddAttributeDropdown(buttonRect, keyValuePairs, onChange);
                },

                onRemoveCallback = (ReorderableList list) =>
                {
                    if (list.index >= 0 && list.index < keyValuePairs.arraySize)
                    {
                        keyValuePairs.DeleteArrayElementAtIndex(list.index);
                        keyValuePairs.serializedObject.ApplyModifiedProperties();
                    }

                    onChange?.Invoke();
                }
            };

            return reorderableList;
        }

        public static float GetElementHeight(SerializedProperty keyValuePairs, int index)
        {
            SerializedProperty element = keyValuePairs.GetArrayElementAtIndex(index);
            if (element == null) return EditorGUIUtility.singleLineHeight;

            SerializedProperty valueProp = element.FindPropertyRelative("value");
            if (valueProp == null) return EditorGUIUtility.singleLineHeight;

            SerializedProperty serialRefProp = valueProp.FindPropertyRelative("serializedReference");
            if (serialRefProp == null || serialRefProp.propertyType != SerializedPropertyType.ManagedReference)
                return EditorGUIUtility.singleLineHeight;

            if (serialRefProp.managedReferenceValue is CustomAttributeValue)
            {
                return EditorGUI.GetPropertyHeight(serialRefProp, true) - (EditorGUIUtility.singleLineHeight * 2);
            }

            return EditorGUIUtility.singleLineHeight;
        }

        public static void DrawAttributeField(Rect rect, SerializedProperty keyProp, SerializedProperty valueProp)
        {
            SerializedProperty serialRefProp = valueProp.FindPropertyRelative("serializedReference");
            SerializedProperty currentTypeProp = valueProp.FindPropertyRelative("currentType");

            Rect keyRect = new Rect(rect.x, rect.y, rect.width / 3f, EditorGUIUtility.singleLineHeight);
            Rect typeRect = new Rect(rect.x + rect.width / 3f, rect.y, rect.width / 3f, EditorGUIUtility.singleLineHeight);
            Rect fieldRect = new Rect(rect.x + 2f * rect.width / 3f, rect.y, rect.width / 3f, EditorGUIUtility.singleLineHeight);

            string key = EditorGUI.TextField(keyRect, keyProp.stringValue);
            keyProp.stringValue = key;

            Type currentType = !string.IsNullOrEmpty(currentTypeProp.stringValue)
                ? Type.GetType(currentTypeProp.stringValue)
                : null;

            BasicSerializableValuesTypes currentEnum = currentType != null && TypeToValueType.TryGetValue(currentType, out var result)
                ? result
                : BasicSerializableValuesTypes.String;

            BasicSerializableValuesTypes selectedEnum = (BasicSerializableValuesTypes)EditorGUI.EnumPopup(typeRect, currentEnum);

            if (selectedEnum != currentEnum || currentType == null)
            {
                IAttributeValue newValue = GetDefaultValue(selectedEnum);
                serialRefProp.managedReferenceValue = newValue;
                currentTypeProp.stringValue = newValue?.GetType().AssemblyQualifiedName ?? "";
                return;
            }

            IAttributeValue instance = serialRefProp.managedReferenceValue as IAttributeValue;

            if (instance is EnumAttributeValue enumAttr)
            {
                EnumAttributeValue newValue = DrawEnumField(enumAttr, rect, fieldRect, serialRefProp);
                serialRefProp.managedReferenceValue = newValue;
                return;
            }

            if (instance is CustomAttributeValue customAttr)
            {
                DrawCustomField(customAttr, rect, fieldRect, serialRefProp);
                return;
            }

            IAttributeValue newPrimitive = DrawAttributeValueField(instance, rect, fieldRect, serialRefProp);
            serialRefProp.managedReferenceValue = newPrimitive;
        }

        public static SerializableKeyValuePair<string, IAttributeValue> DrawAttributeField(Rect rect, SerializableKeyValuePair<string, IAttributeValue> pair)
        {
            Rect keyRect = new Rect(rect.x, rect.y, rect.width / 3f, EditorGUIUtility.singleLineHeight);
            Rect typeRect = new Rect(rect.x + rect.width / 3f, rect.y, rect.width / 3f, EditorGUIUtility.singleLineHeight);
            Rect valueRect = new Rect(rect.x + 2f * rect.width / 3f, rect.y, rect.width / 3f, EditorGUIUtility.singleLineHeight);

            string key = EditorGUI.TextField(keyRect, pair.key);
            BasicSerializableValuesTypes currentType = GetAttributeType(pair.value?.Value);
            BasicSerializableValuesTypes selectedType = (BasicSerializableValuesTypes)EditorGUI.EnumPopup(typeRect, currentType);

            IAttributeValue instance = pair.value.Value ?? GetDefaultValue(selectedType);
            IAttributeValue newValue = selectedType != currentType
                ? GetDefaultValue(selectedType)
                : DrawValueField(instance, valueRect);

            pair.key = key;
            pair.value.Value = newValue;

            return pair;
        }

        private static IAttributeValue DrawValueField(IAttributeValue value, Rect rect)
        {
            if (value == null) return null;

            if (value is IntAttributeValue intValue)
            {
                intValue.Value = EditorGUI.IntField(rect, intValue.Value);
                return intValue;
            }

            if (value is FloatAttributeValue floatValue)
            {
                floatValue.Value = EditorGUI.FloatField(rect, floatValue.Value);
                return floatValue;
            }

            if (value is StringAttributeValue stringValue)
            {
                stringValue.Value = EditorGUI.TextField(rect, stringValue.Value);
                return stringValue;
            }

            if (value is BoolAttributeValue boolValue)
            {
                boolValue.Value = EditorGUI.Toggle(rect, boolValue.Value);
                return boolValue;
            }

            if (value is CharAttributeValue charValue)
            {
                string text = EditorGUI.TextField(rect, charValue.Value.ToString());
                charValue.Value = string.IsNullOrEmpty(text) ? '\0' : text[0];
                return charValue;
            }

            if (value is GameObjectAttributeValue gameObjectValue)
            {
                gameObjectValue.Value = (GameObject)EditorGUI.ObjectField(rect, gameObjectValue.Value, typeof(GameObject), true);
                return gameObjectValue;
            }

            if (value is UnityObjectAttributeValue unityObjectValue)
            {
                unityObjectValue.Value = EditorGUI.ObjectField(rect, unityObjectValue.Value, typeof(UnityEngine.Object), true);
                return unityObjectValue;
            }

            if (value is ScriptableObjectAttributeValue scriptableValue)
            {
                scriptableValue.Value = (ScriptableObject)EditorGUI.ObjectField(rect, scriptableValue.Value, typeof(ScriptableObject), false);
                return scriptableValue;
            }

            if (value is EnumAttributeValue enumValue && enumValue.IsInitialized)
            {
                enumValue.Set(EditorGUI.EnumPopup(rect, (Enum)enumValue.Value));
                return enumValue;
            }

            if (value is CustomAttributeValue customValue && customValue.Value != null)
            {
                EditorGUI.LabelField(rect, customValue.ValueType?.Name ?? "Custom");
                return customValue;
            }

            return value;
        }

        private static void ShowAddAttributeDropdown(Rect buttonRect, SerializedProperty keyValuePairs, System.Action onChange = null)
        {
            var attributeGroups = LoadAttributesFromDatabase();
            AdvancedDropdownState state = new AdvancedDropdownState();

            AttributeSearchDropdown dropdown = new AttributeSearchDropdown(state, attributeGroups,
            (selectedName, selectedType) =>
            {
                AddAttributeToList(keyValuePairs, selectedName, selectedType);
                onChange?.Invoke();
            },
            () =>
            {
                EditorAttributeDialog.ShowWindow("New Attribute", "Enter attribute name", onConfirmBehaviour: (newAttributeName, type) =>
                {
                    AddAttributeToList(keyValuePairs, newAttributeName, type);
                    onChange?.Invoke();
                });
            });

            dropdown.Show(buttonRect);
        }

        private static void AddAttributeToList(SerializedProperty keyValuePairs, string name, BasicSerializableValuesTypes type)
        {
            if (string.IsNullOrEmpty(name)) return;

            int newIndex = keyValuePairs.arraySize;
            keyValuePairs.InsertArrayElementAtIndex(newIndex);

            SerializedProperty newElement = keyValuePairs.GetArrayElementAtIndex(newIndex);
            SerializedProperty keyProp = newElement.FindPropertyRelative("key");
            SerializedProperty valueProp = newElement.FindPropertyRelative("value");

            SerializedProperty unityRefProp = valueProp.FindPropertyRelative("unityReference");
            SerializedProperty serialRefProp = valueProp.FindPropertyRelative("serializedReference");
            SerializedProperty typeProp = valueProp.FindPropertyRelative("currentType");

            IAttributeValue defaultValue = GetDefaultValue(type);

            keyProp.stringValue = GenerateUniqueAttributeName(name, keyValuePairs);
            typeProp.stringValue = defaultValue?.GetType().AssemblyQualifiedName ?? "";

            if (defaultValue is UnityEngine.Object unityObj)
            {
                unityRefProp.objectReferenceValue = unityObj;
                serialRefProp.managedReferenceValue = null;
            }
            else
            {
                unityRefProp.objectReferenceValue = null;
                serialRefProp.managedReferenceValue = defaultValue;
            }

            keyValuePairs.serializedObject.ApplyModifiedProperties();
        }

        private static EnumAttributeValue DrawEnumField(EnumAttributeValue value, Rect lineRect, Rect fieldRect, SerializedProperty property)
        {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            Rect nameRect = new Rect(fieldRect.x, fieldRect.y, fieldRect.width - 60, singleLineHeight);
            Rect buttonRect = new Rect(fieldRect.x + fieldRect.width - 65, fieldRect.y, 65, singleLineHeight);

            if (!value.IsInitialized)
            {
                GUI.Label(nameRect, "Select Enum Type");

                if (GUI.Button(buttonRect, "Set"))
                {
                    TypeSearchDropdown dropdown = new TypeSearchDropdown(
                        new AdvancedDropdownState(),
                        TypeCache.GetTypesDerivedFrom<Enum>().Where(t => t.IsEnum).ToList(),
                        selectedType =>
                        {
                            property.serializedObject.Update();
                            value.Initialize(selectedType);
                            property.serializedObject.ApplyModifiedProperties();
                        },
                        false, false);

                    dropdown.Show(buttonRect);
                }

                return value;
            }

            Enum selectedValue = EditorGUI.EnumPopup(nameRect, (Enum)value.Value);

            if (GUI.Button(buttonRect, "Change"))
            {
                TypeSearchDropdown dropdown = new TypeSearchDropdown(
                    new AdvancedDropdownState(),
                    TypeCache.GetTypesDerivedFrom<Enum>().Where(t => t.IsEnum).ToList(),
                    selectedType =>
                    {
                        property.serializedObject.Update();
                        value.Initialize(selectedType);
                        property.serializedObject.ApplyModifiedProperties();
                    },
                    false, false);

                dropdown.Show(buttonRect);
            }

            value.Set(selectedValue);
            return value;
        }


        private static EnumAttributeValue DrawEnumField(EnumAttributeValue value, Rect rect)
        {
            Enum selectedValue = EditorGUI.EnumPopup(rect, (Enum)value.Value);
            value.Set(selectedValue);
            return value;
        }

        private static CustomAttributeValue DrawCustomField(CustomAttributeValue value, Rect lineRect, Rect fieldRect)
        {
            GUI.Label(fieldRect, value.ValueType?.Name ?? "Select Custom Type");
            return value;
        }

        private static CustomAttributeValue DrawCustomField(CustomAttributeValue value, Rect lineRect, Rect fieldRect, SerializedProperty property)
        {
            float singleLineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            Rect nameRect = new Rect(fieldRect.x, fieldRect.y, fieldRect.width - 60, singleLineHeight);
            Rect buttonRect = new Rect(fieldRect.x + fieldRect.width - 65, fieldRect.y, 65, singleLineHeight);

            if (!value.IsInitialized || value.ValueType == null)
            {
                if (GUI.Button(buttonRect, "Set"))
                {
                    TypeSearchDropdown dropdown = new TypeSearchDropdown(
                        new AdvancedDropdownState(),
                        GetValidCustomTypes(),
                        selectedType =>
                        {
                            property.serializedObject.Update();
                            value.Initialize(selectedType);
                            EditorUtility.SetDirty(property.serializedObject.targetObject);
                            property.serializedObject.ApplyModifiedProperties();
                        },
                        false, false);
                    dropdown.Show(buttonRect);
                }

                GUI.Label(nameRect, "Select Custom Type");
                return value;
            }

            GUI.Label(nameRect, value.ValueType.Name);

            if (GUI.Button(buttonRect, "Change"))
            {
                TypeSearchDropdown dropdown = new TypeSearchDropdown(
                    new AdvancedDropdownState(),
                    GetValidCustomTypes(),
                    selectedType =>
                    {
                        property.serializedObject.Update();
                        value.Initialize(selectedType);
                        EditorUtility.SetDirty(property.serializedObject.targetObject);
                        property.serializedObject.ApplyModifiedProperties();
                    },
                    false, false);

                dropdown.Show(buttonRect);
            }

            if (value.Value != null && value.ValueType != null)
            {
                Rect valueRect = new Rect(lineRect.x + 15, lineRect.y + singleLineHeight + spacing, lineRect.width, EditorGUI.GetPropertyHeight(property, true));
                EditorGUI.PropertyField(valueRect, property.FindPropertyRelative("value"), true);
            }

            return value;
        }

        private static List<Type> GetValidCustomTypes()
        {
            return TypeCache.GetTypesDerivedFrom<object>()
                .Where(type =>
                    !type.IsAbstract &&
                    !type.IsInterface &&
                    type.GetConstructor(Type.EmptyTypes) != null || type.IsValueType &&
                    !type.ContainsGenericParameters &&
                    !type.IsSubclassOf(typeof(UnityEngine.Object)) &&
                    type.IsVisible &&
                    !type.Name.StartsWith("<") &&
                    !typeof(UnityEngine.Object).IsAssignableFrom(type) &&
                    !type.IsDefined(typeof(ObsoleteAttribute), false) &&
                    type.IsDefined(typeof(SerializableAttribute), false))
                .ToList();
        }

        private static string GenerateUniqueAttributeName(string baseName, SerializedProperty keyValuePairs)
        {
            HashSet<string> existingKeys = new HashSet<string>();
            for (int i = 0; i < keyValuePairs.arraySize; i++)
            {
                existingKeys.Add(keyValuePairs.GetArrayElementAtIndex(i).FindPropertyRelative("key").stringValue);
            }

            string uniqueName = baseName;
            int suffix = 1;

            while (existingKeys.Contains(uniqueName))
            {
                uniqueName = $"{baseName} ({suffix++})";
            }

            return uniqueName;
        }

        private static List<KeyValuePair<string, List<KeyValuePair<string, BasicSerializableValuesTypes>>>> LoadAttributesFromDatabase()
        {
            var attributeGroups = new List<KeyValuePair<string, List<KeyValuePair<string, BasicSerializableValuesTypes>>>>();
            string[] databaseGuids = AssetDatabase.FindAssets("t:AttributesDatabase");

            foreach (string guid in databaseGuids)
            {
                AttributesDatabase database = AssetDatabase.LoadAssetAtPath<AttributesDatabase>(AssetDatabase.GUIDToAssetPath(guid));
                if (database != null)
                {
                    var attributes = new List<KeyValuePair<string, BasicSerializableValuesTypes>>();
                    foreach (var attribute in database.NamedDatas)
                    {
                        BasicSerializableValuesTypes type = GetAttributeType(attribute.Value);
                        attributes.Add(new KeyValuePair<string, BasicSerializableValuesTypes>(attribute.Key, type));
                    }

                    attributeGroups.Add(new KeyValuePair<string, List<KeyValuePair<string, BasicSerializableValuesTypes>>>(database.name, attributes));
                }
            }

            return attributeGroups;
        }
        #endregion
    }
}