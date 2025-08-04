using UnityEditor;
using UnityEngine;
using System;

namespace Gamegaard.SerializableAttributes.Editor
{
    public class EditorAttributeDialog : EditorWindow
    {
        private static string attributeName = "";
        private static BasicSerializableValuesTypes selectedType = BasicSerializableValuesTypes.String;
        private static string titleText = "Create Attribute";
        private static string messageText = "Enter attribute details:";
        private static Action<string, BasicSerializableValuesTypes> onConfirm;
        private static Action onCancel;

        public static void ShowWindow(string title, string message, string defaultName = "", BasicSerializableValuesTypes defaultType = BasicSerializableValuesTypes.String, Action<string, BasicSerializableValuesTypes> onConfirmBehaviour = null, Action onCancelBehaviour = null)
        {
            titleText = title;
            messageText = message;
            attributeName = defaultName;
            selectedType = defaultType;
            onConfirm = onConfirmBehaviour;
            onCancel = onCancelBehaviour;

            EditorAttributeDialog window = GetWindow<EditorAttributeDialog>(titleText);
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 300, 150);
            window.ShowModal();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField(messageText, EditorStyles.wordWrappedLabel);

            GUILayout.Space(10);
            attributeName = EditorGUILayout.TextField("Attribute Name:", attributeName);
            selectedType = (BasicSerializableValuesTypes)EditorGUILayout.EnumPopup("Attribute Type:", selectedType);

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Confirm"))
            {
                if (!string.IsNullOrEmpty(attributeName))
                {
                    onConfirm?.Invoke(attributeName, selectedType);
                }
                else
                {
                    EditorUtility.DisplayDialog("Invalid Input", "Attribute name cannot be empty!", "OK");
                }
                Close();
            }
            if (GUILayout.Button("Cancel"))
            {
                onCancel?.Invoke();
                Close();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
