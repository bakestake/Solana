using UnityEngine;
using UnityEditor;
using System;

public class InputWindow : EditorWindow
{
    [SerializeField] private string inputText = "";

    private Action<string> onInputSubmitted;
    private string instructionText = "Enter a value:";
    private string inputLabel = "Input";
    private string confirmButtonText = "Confirm";
    private string cancelButtonText = "Cancel";

    public static void ShowWindow(string windowTitle = "Input Window", string instructionText = "Enter a value:", string inputLabel = "Input", string confirmButtonText = "Confirm", string cancelButtonText = "Cancel", Action<string> onInputSubmitted = null)
    {
        var window = CreateInstance<InputWindow>();
        window.titleContent = new GUIContent(windowTitle);
        window.onInputSubmitted = onInputSubmitted;
        window.instructionText = instructionText;
        window.inputLabel = inputLabel;
        window.confirmButtonText = confirmButtonText;
        window.cancelButtonText = cancelButtonText;

        window.minSize = new Vector2(250, 80);
        window.maxSize = new Vector2(250, 80);

        window.ShowUtility();
    }

    private void OnGUI()
    {
        GUILayout.Label(instructionText, EditorStyles.label);
        inputText = EditorGUILayout.TextField(inputLabel, inputText);

        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(confirmButtonText))
        {
            onInputSubmitted?.Invoke(inputText);
            Close();
        }
        if (GUILayout.Button(cancelButtonText))
        {
            Close();
        }
        GUILayout.EndHorizontal();
    }
}
