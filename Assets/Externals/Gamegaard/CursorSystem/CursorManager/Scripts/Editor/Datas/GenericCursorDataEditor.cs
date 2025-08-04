using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Gamegaard.CursorSystem.Editor
{
    public class GenericCursorDataEditor<T> : UnityEditor.Editor where T : Object
    {
        private SerializedProperty uiIconProperty;
        private SerializedProperty priorityProperty;
        private SerializedProperty defaultStateProperty;
        private SerializedProperty customStatesProperty;
        private SerializedProperty nameProperty;
        private SerializedProperty hotspotProperty;
        private SerializedProperty fpsProperty;

        private ReorderableList defaultFramesList;
        private ReorderableList customStatesList;

        private void OnEnable()
        {
            priorityProperty = serializedObject.FindProperty("priority");
            uiIconProperty = serializedObject.FindProperty("uiIcon");
            defaultStateProperty = serializedObject.FindProperty("defaultState");
            customStatesProperty = serializedObject.FindProperty("customStates");
            nameProperty = defaultStateProperty.FindPropertyRelative("name");
            hotspotProperty = defaultStateProperty.FindPropertyRelative("hotspot");
            fpsProperty = defaultStateProperty.FindPropertyRelative("fps");
            SetupCustomStatesList();
            SetupDefaultFramesList();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawBasicSettings();
            DrawDefaultState();
            customStatesList.DoLayoutList();

            ValidateStates();
            DrawCursorPreview((CursorData2D<T>)target);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawBasicSettings()
        {
            GUIStyle boxStyle = new GUIStyle(EditorStyles.helpBox);
            GUILayout.BeginVertical(boxStyle);
            EditorGUILayout.LabelField(new GUIContent("Basic settings", "All basic settings."), EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(priorityProperty, new GUIContent("Priority", "Determines the priority of this cursor data."));
            EditorGUILayout.PropertyField(uiIconProperty, new GUIContent("UI Icon", "Used to display the cursor to select in options."));
            GUILayout.EndVertical();
        }

        private void DrawDefaultState()
        {
            GUIStyle boxStyle = new GUIStyle(EditorStyles.helpBox);
            GUILayout.BeginVertical(boxStyle);
            EditorGUILayout.LabelField(new GUIContent("Default State", "Defines the default cursor state."), EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("State Name", "The name of the default state."));
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField(nameProperty.stringValue);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(hotspotProperty, new GUIContent("Hotspot", "Defines the position of the cursor's anchor point."));
            GUI.enabled = defaultFramesList.count > 1;
            EditorGUILayout.PropertyField(fpsProperty, new GUIContent("FPS", "Defines the frames per second for animation."));
            GUI.enabled = true;

            defaultFramesList.DoLayoutList();

            GUILayout.EndVertical();
        }

        private void SetupDefaultFramesList()
        {
            SerializedProperty framesProperty = defaultStateProperty.FindPropertyRelative("frames");

            defaultFramesList = new ReorderableList(serializedObject, framesProperty, true, true, true, true);

            defaultFramesList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, new GUIContent("Frames", "Defines the frames for the default state animation."));
            };

            defaultFramesList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                SerializedProperty frameProperty = framesProperty.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    frameProperty,
                    GUIContent.none
                );
            };

            defaultFramesList.onAddCallback = list =>
            {
                framesProperty.arraySize++;
                SerializedProperty newFrame = framesProperty.GetArrayElementAtIndex(framesProperty.arraySize - 1);
                newFrame.objectReferenceValue = null;
            };

            defaultFramesList.onRemoveCallback = list =>
            {
                if (framesProperty.arraySize > 1)
                {
                    framesProperty.DeleteArrayElementAtIndex(list.index);
                }
            };
        }

        private void SetupCustomStatesList()
        {
            customStatesList = new ReorderableList(serializedObject, customStatesProperty, true, true, true, true);

            customStatesList.drawHeaderCallback = rect =>
            {
                float labelWidth = rect.width * 0.8f;
                float fieldWidth = 50f;
                float offsetX = rect.x + rect.width - fieldWidth;

                EditorGUI.LabelField(
                    new Rect(rect.x, rect.y, labelWidth, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Custom States", "Defines additional cursor states."));

                int newSize = EditorGUI.DelayedIntField(
                    new Rect(offsetX, rect.y, fieldWidth, EditorGUIUtility.singleLineHeight),
                    customStatesProperty.arraySize);

                if (newSize != customStatesProperty.arraySize && newSize >= 0)
                {
                    while (customStatesProperty.arraySize < newSize)
                    {
                        AddCustomState();
                    }

                    while (customStatesProperty.arraySize > newSize)
                    {
                        customStatesProperty.DeleteArrayElementAtIndex(customStatesProperty.arraySize - 1);
                    }
                }
            };

            customStatesList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                SerializedProperty stateProperty = customStatesProperty.GetArrayElementAtIndex(index);

                SerializedProperty nameProperty = stateProperty.FindPropertyRelative("name");
                SerializedProperty hotspotProperty = stateProperty.FindPropertyRelative("hotspot");
                SerializedProperty fpsProperty = stateProperty.FindPropertyRelative("fps");
                SerializedProperty framesProperty = stateProperty.FindPropertyRelative("frames");

                float lineHeight = EditorGUIUtility.singleLineHeight;
                float spacing = EditorGUIUtility.standardVerticalSpacing;

                float indent = 15f;
                rect.x += indent;
                rect.width -= indent;

                bool isExpanded = stateProperty.isExpanded;
                stateProperty.isExpanded = EditorGUI.Foldout(
                    new Rect(rect.x, rect.y, rect.width, lineHeight),
                    isExpanded,
                    nameProperty.stringValue);

                if (isExpanded)
                {
                    rect.y += lineHeight + spacing;

                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, rect.width, lineHeight),
                        nameProperty,
                        new GUIContent("State Name", "The name of the custom state."));

                    rect.y += lineHeight + spacing;

                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, rect.width, lineHeight),
                        hotspotProperty,
                        new GUIContent("Hotspot", "Defines the position of the cursor's anchor point."));

                    rect.y += lineHeight + spacing;

                    if (framesProperty.arraySize > 1)
                    {
                        EditorGUI.PropertyField(
                            new Rect(rect.x, rect.y, rect.width, lineHeight),
                            fpsProperty,
                            new GUIContent("FPS", "Defines the frames per second for animation."));
                        rect.y += lineHeight + spacing;
                    }

                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y, rect.width, EditorGUI.GetPropertyHeight(framesProperty, true)),
                        framesProperty,
                        new GUIContent("Frames", "Defines the frames for the custom state animation."),
                        true);
                }
            };

            customStatesList.elementHeightCallback = index =>
            {
                SerializedProperty stateProperty = customStatesProperty.GetArrayElementAtIndex(index);

                if (!stateProperty.isExpanded)
                {
                    return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                SerializedProperty framesProperty = stateProperty.FindPropertyRelative("frames");

                float baseHeight = (3 * EditorGUIUtility.singleLineHeight) + (4 * EditorGUIUtility.standardVerticalSpacing);
                float framesHeight = EditorGUI.GetPropertyHeight(framesProperty, true);

                return framesProperty.arraySize > 1 ? baseHeight + framesHeight + EditorGUIUtility.singleLineHeight : baseHeight + framesHeight;
            };

            customStatesList.onAddCallback = list => AddCustomState();
        }

        private void AddCustomState()
        {
            customStatesProperty.arraySize++;
            SerializedProperty newState = customStatesProperty.GetArrayElementAtIndex(customStatesProperty.arraySize - 1);

            newState.FindPropertyRelative("name").stringValue = customStatesProperty.arraySize == 1 ? "Click" : "New State";
            newState.FindPropertyRelative("hotspot").vector2Value = hotspotProperty.vector2Value;
            newState.FindPropertyRelative("fps").floatValue = 4;
            SerializedProperty frames = newState.FindPropertyRelative("frames");
            frames.arraySize = 1;
            frames.GetArrayElementAtIndex(0).objectReferenceValue = null;
        }

        private void ValidateStates()
        {
            CursorData2D<T> cursorData = (CursorData2D<T>)target;
            HashSet<string> stateNames = new HashSet<string>();
            List<string> duplicateNames = new List<string>();
            List<string> emptyNameStates = new List<string>();
            List<string> emptyFramesStates = new List<string>();

            if (cursorData.DefaultState.Frames == null || cursorData.DefaultState.Frames.Length == 0)
            {
                EditorGUILayout.HelpBox("Default state must have at least one valid frame.", MessageType.Error);
            }

            foreach (CursorState<T> state in cursorData.CustomStates)
            {
                if (string.IsNullOrEmpty(state.Name))
                {
                    emptyNameStates.Add("Unnamed State");
                }
                else if (!stateNames.Add(state.Name))
                {
                    duplicateNames.Add(state.Name);
                }

                if (state.Frames == null || state.Frames.Length == 0 || state.Frames.Any(frame => frame == null))
                {
                    emptyFramesStates.Add(state.Name);
                }
            }

            if (duplicateNames.Any())
            {
                EditorGUILayout.HelpBox($"Duplicate state names: {string.Join(", ", duplicateNames)}.", MessageType.Error);
            }

            if (emptyNameStates.Any())
            {
                EditorGUILayout.HelpBox($"States with empty names: {string.Join(", ", emptyNameStates)}.", MessageType.Error);
            }

            if (emptyFramesStates.Any())
            {
                EditorGUILayout.HelpBox($"States with empty or invalid frames: {string.Join(", ", emptyFramesStates)}.", MessageType.Error);
            }
        }

        private void DrawCursorPreview(CursorData2D<T> cursorData)
        {
            if (cursorData == null || cursorData.DefaultState.Frames == null || cursorData.DefaultState.Frames.Length == 0) return;

            CursorState<T> state = cursorData.DefaultState;
            Object cursorObject = state.Frames[0];

            float previewSize = 64;
            Rect previewRect = GUILayoutUtility.GetRect(previewSize, previewSize, GUILayout.ExpandWidth(false));

            if (cursorObject != null)
            {
                if (cursorObject is Texture2D texture)
                {
                    GUI.DrawTexture(previewRect, texture, ScaleMode.ScaleToFit);
                }
                else if (cursorObject is Sprite sprite)
                {
                    GUI.DrawTexture(previewRect, sprite.texture, ScaleMode.ScaleToFit);
                }
            }

            Vector2 anchorPosition = new Vector2(
                previewRect.xMin + state.Hotspot.x * previewRect.width,
                previewRect.yMin + state.Hotspot.y * previewRect.height
            );

            Handles.color = Color.red;
            Handles.DrawSolidDisc(anchorPosition, Vector3.forward, 4f);

            Handles.color = Color.black;
            Handles.DrawAAPolyLine(2, new Vector3[]
            {
                new Vector3(previewRect.xMin, previewRect.yMin),
                new Vector3(previewRect.xMax, previewRect.yMin),
                new Vector3(previewRect.xMax, previewRect.yMax),
                new Vector3(previewRect.xMin, previewRect.yMax),
                new Vector3(previewRect.xMin, previewRect.yMin)
            });
        }
    }
}
