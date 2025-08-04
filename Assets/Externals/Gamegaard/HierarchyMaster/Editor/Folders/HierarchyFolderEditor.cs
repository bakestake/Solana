using UnityEditor;
using UnityEngine;

namespace Gamegaard.HierarchyMaster.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(HierarchyFolder))]
    public class HierarchyFolderEditor : UnityEditor.Editor
    {
        private HierarchyFolder folderTarget;
        private static bool wasInteracting = false;
        private static Vector3 lastPosition;
        private static Quaternion lastRotation;
        private static Vector3 lastScale;
        private static Matrix4x4 initialMatrix;

        private void OnEnable()
        {
            folderTarget = (HierarchyFolder)target;

            if (!HierarchyMasterSettings.Instance.ShowTransformComponent)
            {
                folderTarget.transform.hideFlags = HideFlags.HideInInspector;
            }
            else
            {
                folderTarget.transform.hideFlags = HideFlags.None;
            }

            Tools.hidden = HierarchyMasterSettings.Instance.LockTools;
        }

        private void OnDisable()
        {
            Tools.hidden = false;
        }

        private void OnSceneGUI()
        {
            Handles.BeginGUI();
            Handles.EndGUI();

            Transform t = folderTarget.transform;

            if (Tools.current == Tool.Move || Tools.current == Tool.Rotate || Tools.current == Tool.Scale)
            {
                if (!wasInteracting)
                {
                    lastPosition = t.position;
                    lastRotation = t.rotation;
                    lastScale = t.localScale;
                    initialMatrix = Matrix4x4.TRS(t.position, t.rotation, t.localScale);
                    wasInteracting = true;
                }

                if (wasInteracting && GUIUtility.hotControl == 0)
                {
                    if (t.position != lastPosition || t.rotation != lastRotation || t.localScale != lastScale)
                    {
                        Matrix4x4 currentMatrix = Matrix4x4.TRS(t.position, t.rotation, t.localScale);
                        Matrix4x4 delta = currentMatrix * initialMatrix.inverse;

                        if (HierarchyMasterSettings.Instance.ForceZeroTransform)
                        {
                            ApplyDeltaToChildren(t, delta);
                            Undo.RegisterFullObjectHierarchyUndo(t, "Reset Folder Transform");
                            ResetTransform(t);
                        }
                    }

                    wasInteracting = false;
                }
            }
        }

        private void ApplyDeltaToChildren(Transform parent, Matrix4x4 delta)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                Undo.RecordObject(child, "Apply Folder Transform");

                Vector3 newPosition = delta.MultiplyPoint3x4(child.position);
                Quaternion newRotation = delta.rotation * child.rotation;
                Vector3 newScale = Vector3.Scale(delta.lossyScale, child.localScale);

                child.SetPositionAndRotation(newPosition, newRotation);
                child.localScale = newScale;
            }
        }

        private void ResetTransform(Transform t)
        {
            t.position = Vector3.zero;
            t.rotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }

        public override void OnInspectorGUI()
        {
            GUIStyle infoStyle = new GUIStyle(EditorStyles.helpBox)
            {
                wordWrap = true,
                fontSize = 12,
                fontStyle = FontStyle.Italic,
                padding = new RectOffset(10, 10, 5, 5)
            };

            EditorGUILayout.LabelField(
                "This object is an organizational folder. It will be excluded from the build and will not affect contained objects.",
                infoStyle,
                GUILayout.ExpandWidth(true)
            );

            EditorGUILayout.Space();

            SerializedProperty useCustomIconsProp = serializedObject.FindProperty("useCustomIcons");
            EditorGUILayout.PropertyField(useCustomIconsProp, new GUIContent("Use Custom Icons"));

            if (useCustomIconsProp.boolValue)
            {
                SerializedProperty customOpenProp = serializedObject.FindProperty("customOpenFolderIcon");
                SerializedProperty customClosedProp = serializedObject.FindProperty("customClosedFolderIcon");

                EditorGUILayout.PropertyField(customOpenProp, new GUIContent("Custom Open Folder Icon"));
                EditorGUILayout.PropertyField(customClosedProp, new GUIContent("Custom Closed Folder Icon"));

                EditorGUILayout.HelpBox(
                    "If only one icon is set, it will be used for both states.",
                    MessageType.Info
                );
            }

            serializedObject.ApplyModifiedProperties();
        }

        public override bool HasPreviewGUI()
        {
            return true;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (Event.current.type == EventType.MouseDown && r.Contains(Event.current.mousePosition))
            {
                Event.current.Use();
            }
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }
    }
}
