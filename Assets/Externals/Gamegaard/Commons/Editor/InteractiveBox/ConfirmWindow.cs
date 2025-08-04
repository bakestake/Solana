using UnityEngine;
using UnityEditor;
using System;

public class ConfirmWindow : EditorWindow
{
    private string _message = string.Empty;
    private Action _onConfirm;
    private Action _onCancel;

    public static void ShowWindow(string windowTitle = "Confirm", string message = "Are you sure?", Action onConfirm = null, Action onCancel = null)
    {
        ConfirmWindow window = GetWindow<ConfirmWindow>(true, windowTitle, true);
        window._message = message;
        window._onConfirm = onConfirm;
        window._onCancel = onCancel;
        window.minSize = new Vector2(300, 100);
        window.ShowUtility();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField(_message, EditorStyles.wordWrappedLabel);
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Confirm"))
        {
            _onConfirm?.Invoke();
            Close();
        }
        if (GUILayout.Button("Cancel"))
        {
            _onCancel?.Invoke();
            Close();
        }
        GUILayout.EndHorizontal();
    }
}
