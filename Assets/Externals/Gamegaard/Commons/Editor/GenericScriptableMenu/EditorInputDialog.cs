using UnityEditor;
using UnityEngine;
using System;

namespace Gamegaard.Commons.Editor
{
    public class EditorInputDialog : EditorWindow
    {
        private static string input = "";
        private static string titleText = "Input";
        private static string messageText = "Enter value:";
        private static Action<string> onConfirm;
        private static Action onCancel;

        public static string ShowWindow(string title, string message, string defaultInput = "", Action<string> onConfirmBehaviour = null, Action onCancelBehaviour = null)
        {
            titleText = title;
            messageText = message;
            input = defaultInput;
            onConfirm = onConfirmBehaviour;
            onCancel = onCancelBehaviour;

            EditorInputDialog window = GetWindow<EditorInputDialog>(titleText);
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 100);
            window.ShowModal();
            return input;
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField(messageText, EditorStyles.wordWrappedLabel);
            input = EditorGUILayout.TextField(input);

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Confirm"))
            {
                onConfirm?.Invoke(input);
                Close();
            }
            if (GUILayout.Button("Cancel"))
            {
                onCancel?.Invoke();
                input = null;
                Close();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
