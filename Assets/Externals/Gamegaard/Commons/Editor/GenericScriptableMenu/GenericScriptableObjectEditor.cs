using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Gamegaard.Commons.Editor.Utils;

namespace Gamegaard.Commons.Editor
{
    public abstract class GenericScriptableObjectEditor<T> : EditorWindow where T : ScriptableObject
    {
        protected Vector2 rightPanelScroll;
        protected Vector2 leftPanelScroll;
        protected T currentSelectedObject;
        protected int selectedIndex = -1;
        protected int selectedTab = 0;
        private ReorderableList reorderableList;
        private List<int> selectedIndices = new List<int>();

        private const float minLeftPanelWidth = 200f;
        private const float maxLeftPanelWidth = 400f;
        private float leftPanelWidth = 300f;
        private int lastSelectionIndex = -1;
        private bool expandedUpward = false;

        private string searchQuery = "";
        private int selectedSearchMode = 0;
        private IObjectEditorHelper<T> currentSearchHelper;

        protected abstract string[] Tabs { get; }
        protected abstract string Title { get; }
        protected virtual Dictionary<string, IObjectEditorHelper<T>> SearchModes { get; set; } = new Dictionary<string, IObjectEditorHelper<T>>
        {
            { "Simple Search", new ObjectGroupingHelper<T>() }
        };

        protected abstract void DrawDetails(SerializedObject serializedObject);
        protected virtual void DrawTabContent(int tabIndex, SerializedObject serializedObject) { }

        protected virtual void OnEnable()
        {
            if (position.width == 0 && position.height == 0)
            {
                minSize = new Vector2(600, 500);
                position = new Rect(position.x, position.y, 600, 500);
            }

            InitializeSearchHelper();
            InitializeReorderableList();
        }

        protected virtual void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayoutOption expandHeight = GUILayout.ExpandHeight(true);

            // Left Panel
            EditorGUILayout.BeginVertical(GUILayout.Width(leftPanelWidth), expandHeight);
            GUIStyle darkBackground = new GUIStyle(GUI.skin.box)
            {
                normal = { background = MakeTex(1, 1, new Color(0.2f, 0.2f, 0.2f)) }
            };

            EditorGUILayout.BeginVertical(darkBackground, expandHeight);
            GamegaardGUIUtils.DrawBigTitleText(Title);

            // Tabs
            selectedTab = GUILayout.Toolbar(selectedTab, Tabs);
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            DrawSearchField();
            DrawSearchModeDropdown();
            EditorGUILayout.EndHorizontal();

            // Reorderable List
            leftPanelScroll = EditorGUILayout.BeginScrollView(leftPanelScroll);
            reorderableList.DoLayoutList();
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();

            // Resizer
            ResizeLeftPanel();

            // Right Panel
            rightPanelScroll = EditorGUILayout.BeginScrollView(rightPanelScroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            if (currentSelectedObject != null)
            {
                SerializedObject serializedObject = new SerializedObject(currentSelectedObject);
                serializedObject.Update();

                // Draw Tab Content
                if (selectedTab == 0)
                {
                    DrawDetails(serializedObject);
                }
                else
                {
                    DrawTabContent(selectedTab, serializedObject);
                }

                serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
        }

        private void InitializeSearchHelper()
        {
            currentSearchHelper = SearchModes.ElementAt(selectedSearchMode).Value;
            currentSearchHelper.Initialize(LoadObjects());
        }

        protected virtual Dictionary<string, T> LoadObjects()
        {
            return AssetDatabase.FindAssets($"t:{typeof(T).Name}")
                .Select(guid =>
                {
                    var asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
                    return new KeyValuePair<string, T>(asset.name, asset);
                })
                .ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        private void InitializeReorderableList()
        {
            reorderableList = new ReorderableList(currentSearchHelper.FilteredObjects.Keys.ToList(), typeof(string), true, true, true, true);
            reorderableList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, $"{typeof(T).Name}s");
            };
            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (index < currentSearchHelper.FilteredObjects.Keys.Count)
                {
                    string key = currentSearchHelper.FilteredObjects.Keys.ToList()[index];
                    bool isSelected = selectedIndices.Contains(index);

                    if (isSelected)
                    {
                        EditorGUI.DrawRect(rect, new Color(0.24f, 0.49f, 0.8f, 0.5f));
                    }
                    EditorGUI.LabelField(rect, key);

                    if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                    {
                        if (Event.current.button == 1)
                        {
                            ShowContextMenu(key, currentSearchHelper.FilteredObjects[key]);
                            Event.current.Use();
                            return;
                        }

                        if (Event.current.button == 0)
                        {
                            if (Event.current.control || Event.current.command)
                            {
                                if (isSelected)
                                {
                                    selectedIndices.Remove(index);
                                }
                                else
                                {
                                    selectedIndices.Add(index);
                                }
                            }
                            else if (Event.current.shift)
                            {
                                if (lastSelectionIndex == -1)
                                {
                                    lastSelectionIndex = index;
                                }

                                expandedUpward = index < lastSelectionIndex;

                                if (isSelected)
                                {
                                    if (expandedUpward)
                                    {
                                        for (int i = index; i <= lastSelectionIndex; i++)
                                        {
                                            selectedIndices.Remove(i);
                                        }
                                    }
                                    else
                                    {
                                        for (int i = lastSelectionIndex; i <= index; i++)
                                        {
                                            selectedIndices.Remove(i);
                                        }
                                    }
                                }
                                else
                                {
                                    int rangeStart = Mathf.Min(index, lastSelectionIndex);
                                    int rangeEnd = Mathf.Max(index, lastSelectionIndex);

                                    for (int i = rangeStart; i <= rangeEnd; i++)
                                    {
                                        selectedIndices.Add(i);
                                    }
                                }

                                lastSelectionIndex = index;
                            }
                            else
                            {
                                selectedIndices.Clear();
                                selectedIndices.Add(index);
                                lastSelectionIndex = index;

                                if (index >= 0 && index < currentSearchHelper.FilteredObjects.Count)
                                {
                                    string nKey = currentSearchHelper.FilteredObjects.Keys.ToList()[index];
                                    currentSelectedObject = currentSearchHelper.FilteredObjects[key];
                                }
                            }

                            Repaint();
                            Event.current.Use();
                        }
                    }
                }
            };

            reorderableList.elementHeightCallback = (index) =>
            {
                return EditorGUIUtility.singleLineHeight + 4f;
            };
        }

        #region ContextMenu
        private void DeleteSelectedObjects()
        {
            foreach (int index in selectedIndices.ToList())
            {
                string key = currentSearchHelper.FilteredObjects.Keys.ToList()[index];
                RemoveObject(key);
            }
            selectedIndices.Clear();
            Repaint();
        }

        private void DuplicateSelectedObjects()
        {
            foreach (int index in selectedIndices)
            {
                string key = currentSearchHelper.FilteredObjects.Keys.ToList()[index];
                DuplicateObject(key, currentSearchHelper.FilteredObjects[key]);
            }
            selectedIndices.Clear();
            Repaint();
        }

        private void ShowContextMenu(string key, T selectedObject)
        {
            GenericMenu menu = new GenericMenu();

            if (selectedIndices.Count > 1)
            {
                ShowMultipleObjectsContextMenu(key, menu);
            }
            else
            {
                ShowSingleObjectContextMenu(key, selectedObject, menu);
            }

            menu.ShowAsContext();
        }

        protected virtual void ShowMultipleObjectsContextMenu(string key, GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Delete Selected"), false, DeleteSelectedObjects);
            menu.AddItem(new GUIContent("Duplicate Selected"), false, DuplicateSelectedObjects);
        }

        protected virtual void ShowSingleObjectContextMenu(string key, T selectedObject, GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Select in Inspector"), false, () => HighlightInInspector(selectedObject));
            menu.AddItem(new GUIContent("Rename"), false, () => RenameObject(key, selectedObject));
            menu.AddItem(new GUIContent("Duplicate"), false, () => DuplicateObject(key, selectedObject));
            menu.AddItem(new GUIContent("Remove"), false, () => RemoveObject(key));
            menu.AddItem(new GUIContent("Copy"), false, () => CopyObject(selectedObject));
        }

        private void HighlightInInspector(T selectedObject)
        {
            if (selectedObject != null)
            {
                Selection.activeObject = selectedObject;
                EditorGUIUtility.PingObject(selectedObject);
            }
        }

        private void RenameObject(string key, T selectedObject)
        {
            if (selectedObject == null) return;

            string path = AssetDatabase.GetAssetPath(selectedObject);
            if (string.IsNullOrEmpty(path)) return;

            string newName = EditorInputDialog.ShowWindow("Rename Asset", $"Current Name: {key}\nEnter a new name:", key, null);

            if (!string.IsNullOrEmpty(newName) && newName != key)
            {
                AssetDatabase.RenameAsset(path, newName);
                AssetDatabase.SaveAssets();

                currentSearchHelper.TrackedObjects.Remove(key);
                currentSearchHelper.TrackedObjects[newName] = selectedObject;

                ApplySearchFilter();
            }
        }

        private void DuplicateObject(string key, T selectedObject)
        {
            if (selectedObject == null) return;

            string path = AssetDatabase.GetAssetPath(selectedObject);
            if (string.IsNullOrEmpty(path)) return;

            string newPath = AssetDatabase.GenerateUniqueAssetPath(path);
            AssetDatabase.CopyAsset(path, newPath);
            AssetDatabase.SaveAssets();

            AddNewObject(AssetDatabase.LoadAssetAtPath<T>(newPath));
        }

        private void RemoveObject(string key)
        {
            string path = AssetDatabase.GetAssetPath(currentSearchHelper.FilteredObjects[key]);
            AssetDatabase.DeleteAsset(path);
            currentSearchHelper.FilteredObjects.Remove(key);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Repaint();
        }

        private void CopyObject(T selectedObject)
        {
            EditorGUIUtility.systemCopyBuffer = selectedObject.name;
        }

        private void AddNewObject(T newObject)
        {
            if (newObject == null) return;

            string key = newObject.name;
            currentSearchHelper.TrackedObjects[key] = newObject;

            ApplySearchFilter();
        }
        #endregion

        private void DrawSearchModeDropdown()
        {
            int newSearchMode = EditorGUILayout.Popup(selectedSearchMode, SearchModes.Keys.ToArray(), GUILayout.Width(100));
            if (newSearchMode != selectedSearchMode)
            {
                selectedSearchMode = newSearchMode;
                InitializeSearchHelper();
                ApplySearchFilter();
            }
        }

        private void DrawSearchField()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Search:", GUILayout.Width(50));
            string newSearchQuery = EditorGUILayout.TextField(searchQuery);
            if (newSearchQuery != searchQuery)
            {
                searchQuery = newSearchQuery;
                ApplySearchFilter();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ApplySearchFilter()
        {
            currentSearchHelper.ApplySearchFilter(searchQuery);
            reorderableList.list = currentSearchHelper.FilteredObjects.Keys.ToList();
        }

        private void ResizeLeftPanel()
        {
            Rect resizer = new Rect(leftPanelWidth - 5, 0, 10, position.height);
            EditorGUIUtility.AddCursorRect(resizer, MouseCursor.ResizeHorizontal);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && resizer.Contains(Event.current.mousePosition))
            {
                GUIUtility.hotControl = GUIUtility.GetControlID(FocusType.Passive);
                Event.current.Use();
            }

            if (Event.current.type == EventType.MouseUp && GUIUtility.hotControl != 0)
            {
                GUIUtility.hotControl = 0;
                Event.current.Use();
            }

            if (GUIUtility.hotControl != 0 && Event.current.type == EventType.MouseDrag)
            {
                leftPanelWidth = Mathf.Clamp(Event.current.mousePosition.x, minLeftPanelWidth, maxLeftPanelWidth);
                Repaint();
                Event.current.Use();
            }
        }

        protected Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}