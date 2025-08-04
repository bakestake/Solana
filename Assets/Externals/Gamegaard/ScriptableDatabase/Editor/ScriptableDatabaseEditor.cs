using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using UnityEditorInternal;
using System.Collections.Generic;
using Gamegaard.CustomValues.Database;

[CustomEditor(typeof(ScriptableDatabase<>), true)]
public class ScriptableDatabaseEditor : Editor
{
    private enum SortMode
    {
        Name,
        Type
    }

    private SerializedProperty datasProperty;
    private Type genericArgumentType;

    private SortMode currentSortMode = SortMode.Name;
    private string searchKeyword = string.Empty;
    private ReorderableList reorderableList;

    private void OnEnable()
    {
        datasProperty = serializedObject.FindProperty("datas");
        genericArgumentType = GetGenericArgumentType(target.GetType());
        InitializeReorderableList();
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();
        DrawButtonsArea();
        EditorGUILayout.Space();
        DrawSearchField();
        EditorGUILayout.Space();
        DrawMessageField();
        EditorGUILayout.Space();

        serializedObject.Update();
        HandleDragAndDropOnList();
        reorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

    private void HandleDragAndDropOnList()
    {
        Event evt = Event.current;

        Rect listRect = GUILayoutUtility.GetLastRect();
        listRect.height = reorderableList.headerHeight;

        if (evt.type == EventType.DragUpdated && listRect.Contains(evt.mousePosition))
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            evt.Use();
        }

        if (evt.type == EventType.DragPerform && listRect.Contains(evt.mousePosition))
        {
            DragAndDrop.AcceptDrag();
            foreach (var draggedObject in DragAndDrop.objectReferences)
            {
                if (genericArgumentType.IsInstanceOfType(draggedObject))
                {
                    datasProperty.InsertArrayElementAtIndex(datasProperty.arraySize);
                    datasProperty.GetArrayElementAtIndex(datasProperty.arraySize - 1).objectReferenceValue = draggedObject;
                }
            }
            serializedObject.ApplyModifiedProperties();
            evt.Use();
        }
    }


    private void InitializeReorderableList()
    {
        reorderableList = new ReorderableList(datasProperty.serializedObject, datasProperty, true, true, true, true);

        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 60, rect.height), $"{genericArgumentType.Name}s");

            int newSize = EditorGUI.DelayedIntField(
                new Rect(rect.x + rect.width - 60, rect.y, 50, EditorGUIUtility.singleLineHeight),
                datasProperty.arraySize
            );

            SortMode newSortMode = (SortMode)EditorGUI.EnumPopup(
                new Rect(rect.x + rect.width - 160, rect.y, 90, EditorGUIUtility.singleLineHeight),
                currentSortMode
            );

            if (newSize != datasProperty.arraySize && newSize >= 0)
            {
                datasProperty.arraySize = newSize;
                serializedObject.ApplyModifiedProperties();
            }

            if (newSortMode != currentSortMode)
            {
                currentSortMode = newSortMode;
                SortDatabase(currentSortMode.ToString());
            }
        };

        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = datasProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element, GUIContent.none);
        };

        ApplySearchFilter();
    }

    private void DrawMessageField()
    {
        if (datasProperty != null)
        {
            int totalItems = datasProperty.arraySize;
            int nullCount = CountNullReferences();
            int duplicateCount = CountDuplicates();

            if (nullCount > 0)
            {
                EditorGUILayout.HelpBox($"There are {nullCount} null references in the database.", MessageType.Warning);
            }
            if (duplicateCount > 0)
            {
                EditorGUILayout.HelpBox($"There are {duplicateCount} duplicate references in the database.", MessageType.Warning);
            }
        }
    }

    private void DrawSearchField()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Search:", GUILayout.Width(50));
        string newSearchQuery = EditorGUILayout.TextField(searchKeyword);
        if (newSearchQuery != searchKeyword)
        {
            searchKeyword = newSearchQuery;
            ApplySearchFilter();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawButtonsArea()
    {
        GUILayout.BeginHorizontal();
        if (typeof(UnityEngine.Object).IsAssignableFrom(genericArgumentType))
        {
            if (GUILayout.Button("Find All"))
            {
                PopulateDatabaseWithAssets(genericArgumentType);
                ApplySearchFilter();
            }
        }
        else
        {
            using (new EditorGUI.DisabledScope(true))
            {
                GUILayout.Button(new GUIContent("Find All", "This option is only available for UnityEngine.Object types."));
            }
        }

        if (IsNullableType(genericArgumentType))
        {
            if (GUILayout.Button("Remove Null"))
            {
                RemoveNullReferences();
                ApplySearchFilter();
            }
        }
        else
        {
            using (new EditorGUI.DisabledScope(true))
            {
                GUILayout.Button(new GUIContent("Remove Null", "This option is disabled because the type cannot contain null values."));
            }
        }

        if (GUILayout.Button("Export to JSON"))
        {
            ExportToJson();
        }

        if (GUILayout.Button("Import from JSON"))
        {
            ImportFromJson();
            ApplySearchFilter();
        }
        GUILayout.EndHorizontal();
    }

    private bool IsNullableType(Type type)
    {
        return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
    }

    private void PopulateDatabaseWithAssets(Type assetType)
    {
        UnityEngine.Object[] foundObjects = FindAssetsOfType(assetType);

        datasProperty.ClearArray();
        foreach (UnityEngine.Object obj in foundObjects)
        {
            datasProperty.InsertArrayElementAtIndex(datasProperty.arraySize);
            datasProperty.GetArrayElementAtIndex(datasProperty.arraySize - 1).objectReferenceValue = obj;
        }

        serializedObject.ApplyModifiedProperties();
        Debug.Log($"Found {foundObjects.Length} objects of type {assetType.Name}.");
    }

    private UnityEngine.Object[] FindAssetsOfType(Type type)
    {
        var matchingObjects = new List<UnityEngine.Object>();

        if (typeof(ScriptableObject).IsAssignableFrom(type) || typeof(UnityEngine.Object).IsAssignableFrom(type))
        {
            var guids = AssetDatabase.FindAssets($"t:{type.Name}");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(path, type);
                if (asset != null)
                {
                    matchingObjects.Add(asset);
                }
            }
        }

        if (typeof(Component).IsAssignableFrom(type) || typeof(MonoBehaviour).IsAssignableFrom(type))
        {
            string[] guids = AssetDatabase.FindAssets("t:GameObject");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (obj != null)
                {
                    Component component = obj.GetComponent(type);
                    if (component != null)
                    {
                        matchingObjects.Add(component);
                    }
                }
            }
        }

        return matchingObjects.ToArray();
    }

    private void SortDatabase(string criteria)
    {
        var objects = new List<object>();
        for (int i = 0; i < datasProperty.arraySize; i++)
        {
            objects.Add(GetValueAtIndex(i));
        }

        if (criteria == "Name")
        {
            objects = objects
                .Where(obj => obj != null)
                .OrderBy(obj => obj.ToString())
                .ToList();
        }
        else if (criteria == "Type")
        {
            objects = objects
                .Where(obj => obj != null)
                .OrderBy(obj => obj.GetType().Name)
                .ToList();
        }

        datasProperty.ClearArray();
        foreach (var obj in objects)
        {
            AddValueToProperty(obj);
        }

        serializedObject.ApplyModifiedProperties();
        ApplySearchFilter();
    }

    private void RemoveNullReferences()
    {
        for (int i = datasProperty.arraySize - 1; i >= 0; i--)
        {
            if (GetValueAtIndex(i) == null)
            {
                datasProperty.DeleteArrayElementAtIndex(i);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void ExportToJson()
    {
        string json = JsonUtility.ToJson(serializedObject.targetObject, true);
        string path = EditorUtility.SaveFilePanel("Export Database", "", "Database.json", "json");
        if (!string.IsNullOrEmpty(path))
        {
            System.IO.File.WriteAllText(path, json);
        }
    }

    private void ImportFromJson()
    {
        string path = EditorUtility.OpenFilePanel("Import Database", "", "json");
        if (!string.IsNullOrEmpty(path))
        {
            string json = System.IO.File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(json, serializedObject.targetObject);
            serializedObject.Update();
        }
    }

    private void ApplySearchFilter()
    {
        List<object> filteredList = new List<object>();

        for (int i = 0; i < datasProperty.arraySize; i++)
        {
            object value = GetValueAtIndex(i);
            if (value != null &&
                (string.IsNullOrEmpty(searchKeyword) ||
                 value.ToString().IndexOf(searchKeyword, StringComparison.OrdinalIgnoreCase) >= 0))
            {
                filteredList.Add(value);
            }
        }

        reorderableList.list = filteredList;
        Repaint();
    }

    private int CountNullReferences()
    {
        int count = 0;
        for (int i = datasProperty.arraySize - 1; i >= 0; i--)
        {
            if (GetValueAtIndex(i) == null)
            {
                count++;
            }
        }
        return count;
    }

    private int CountDuplicates()
    {
        var references = new List<object>();
        for (int i = 0; i < datasProperty.arraySize; i++)
        {
            references.Add(GetValueAtIndex(i));
        }

        return references
            .Where(obj => obj != null)
            .GroupBy(obj => obj)
            .Count(group => group.Count() > 1);
    }

    private object GetValueAtIndex(int index)
    {
        SerializedProperty element = datasProperty.GetArrayElementAtIndex(index);
        if (element.propertyType == SerializedPropertyType.ObjectReference)
        {
            return element.objectReferenceValue;
        }
        else if (element.propertyType == SerializedPropertyType.String)
        {
            return element.stringValue;
        }
        else if (element.propertyType == SerializedPropertyType.Integer)
        {
            return element.intValue;
        }
        else if (element.propertyType == SerializedPropertyType.Float)
        {
            return element.floatValue;
        }
        else if (element.propertyType == SerializedPropertyType.Boolean)
        {
            return element.boolValue;
        }
        else if (element.propertyType == SerializedPropertyType.Character)
        {
            return (char)element.intValue;
        }

        return null;
    }

    private void AddValueToProperty(object value)
    {
        datasProperty.InsertArrayElementAtIndex(datasProperty.arraySize);
        SerializedProperty element = datasProperty.GetArrayElementAtIndex(datasProperty.arraySize - 1);

        if (value is UnityEngine.Object unityObject)
        {
            element.objectReferenceValue = unityObject;
        }
        else if (value is string stringValue)
        {
            element.stringValue = stringValue;
        }
        else if (value is int intValue)
        {
            element.intValue = intValue;
        }
        else if (value is float floatValue)
        {
            element.floatValue = floatValue;
        }
        else if (value is bool boolValue)
        {
            element.boolValue = boolValue;
        }
        else if (value is char charValue)
        {
            element.intValue = charValue;
        }
    }

    private Type GetGenericArgumentType(Type databaseType)
    {
        while (databaseType != null && databaseType.BaseType != null)
        {
            if (databaseType.BaseType.IsGenericType && databaseType.BaseType.GetGenericTypeDefinition() == typeof(ScriptableDatabase<>))
            {
                return databaseType.BaseType.GetGenericArguments()[0];
            }
            databaseType = databaseType.BaseType;
        }
        return null;
    }
}
