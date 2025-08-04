using UnityEditor;
using UnityEngine;

namespace Gamegaard.Commons.Editor.Utils
{
    public static class GamegaardGUIUtils
    {
        #region Titles
        /// <summary>
        /// Renders a big title label with the specified text.
        /// </summary>
        /// <param name="text">The text to be displayed in the big title.</param>
        public static void DrawBigTitleText(string text)
        {
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 25;
            EditorGUILayout.LabelField(text, style, GUILayout.Height(style.fontSize + 10));
        }

        /// <summary>
        /// Renders a title label with the specified text.
        /// </summary>
        /// <param name="text">The text to be displayed in the title.</param>
        public static void DrawTitleText(string text)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(text, EditorStyles.boldLabel);
        }

        /// <summary>
        /// Renders a titled label with a horizontal line underneath.
        /// </summary>
        /// <param name="text">The text to be displayed in the title.</param>
        public static void DrawLinedTitle(string text)
        {
            DrawTitleText(text);
            DrawGuiLine();
        }
        #endregion

        #region Lines
        /// <summary>
        /// Renders a horizontal line of the specified height in gray color.
        /// </summary>
        /// <param name="i_height">The height of the line to be rendered.</param>
        public static void DrawGuiLine(int i_height = 1)
        {
            DrawGuiLine(new Color(0.5f, 0.5f, 0.5f, 1), i_height);
        }

        /// <summary>
        /// Renders a horizontal line of the specified height and color.
        /// </summary>
        /// <param name="i_height">The height of the line to be rendered.</param>
        public static void DrawGuiLine(Color color, int i_height = 1)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, i_height);
            DrawGuiLine(rect, color, i_height);
        }

        public static void DrawGuiLine(Rect rect, Color color, int i_height = 1)
        {
            rect.height = i_height;
            EditorGUI.DrawRect(rect, color);
        }

        /// <summary>
        /// Renders a horizontal dashed line of the specified height in gray color.
        /// </summary>
        /// <param name="i_height">The height of the line to be rendered.</param>
        public static void DrawGuiDashedLine(int i_height = 1, float dashSize = 5, float gapSize = 2)
        {
            DrawGuiDashedLine(new Color(0.5f, 0.5f, 0.5f, 1), i_height, dashSize, gapSize);
        }

        /// <summary>
        /// Renders a horizontal dashed line of the specified height and color.
        /// </summary>
        /// <param name="i_height">The height of the line to be rendered.</param>
        public static void DrawGuiDashedLine(Color color, int i_height = 1, float dashSize = 5, float gapSize = 2)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, i_height);
            DrawGuiDashedLine(rect, color, i_height, dashSize, gapSize);
        }

        public static void DrawGuiDashedLine(Rect rect, Color color, int i_height = 1, float dashSize = 5, float gapSize = 2)
        {
            rect.height = i_height;
            Handles.color = color;
            float y = rect.y + rect.height / 2;

            for (float x = rect.x; x < rect.x + rect.width; x += dashSize + gapSize)
            {
                float xEnd = Mathf.Min(x + dashSize, rect.x + rect.width);
                Handles.DrawLine(new Vector3(x, y, 0), new Vector3(xEnd, y, 0));
            }
        }

        /// <summary>
        /// Renders a horizontal dotted line of the specified height in gray color.
        /// </summary>
        /// <param name="i_height">The height of the line to be rendered.</param>
        public static void DrawGuiDottedLine(int i_height = 1, float dotSpacing = 3.5f)
        {
            DrawGuiDottedLine(new Color(0.5f, 0.5f, 0.5f, 1), i_height, dotSpacing);
        }

        /// <summary>
        /// Renders a horizontal dotted line of the specified height and color.
        /// </summary>
        /// <param name="i_height">The height of the line to be rendered.</param>
        public static void DrawGuiDottedLine(Color color, int i_height = 1, float dotSpacing = 3.5f)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, i_height);
            DrawGuiDottedLine(rect, color, i_height, dotSpacing);
        }

        public static void DrawGuiDottedLine(Rect rect, Color color, int i_height = 1, float dotSpacing = 3.5f)
        {
            rect.height = i_height;
            Handles.color = color;
            float y = rect.y + rect.height / 2;

            for (float x = rect.x; x < rect.x + rect.width; x += dotSpacing)
            {
                Handles.DrawSolidDisc(new Vector3(x, y, 0), Vector3.forward, 1f);
            }
        }
        #endregion

        #region Bars
        public static float DrawInteractibleTextedProgressBar(string label, float currentValue, float maxValue, Color fillColor, string valueFormat = "0.00", bool showPercentage = false)
        {
            Rect rect = EditorGUILayout.GetControlRect(false);
            return DrawInteractibleTextedProgressBar(rect, label, currentValue, maxValue, fillColor, valueFormat, showPercentage);
        }

        public static float DrawInteractibleTextedProgressBar(Rect position, string label, float currentValue, float maxValue, Color fillColor, string valueFormat = "0.00", bool showPercentage = false)
        {
            DrawsTextedProgressBar(position, label, currentValue, maxValue, fillColor, valueFormat, showPercentage);
            float newValue = EditorGUI.Slider(position, currentValue, 0f, maxValue);
            return newValue;
        }

        public static int DrawInteractibleTextedProgressBar(string label, int currentValue, int maxValue, Color fillColor)
        {
            Rect rect = EditorGUILayout.GetControlRect(false);
            return DrawInteractibleTextedProgressBar(rect, label, currentValue, maxValue, fillColor);
        }

        public static int DrawInteractibleTextedProgressBar(Rect position, string label, int currentValue, int maxValue, Color fillColor)
        {
            DrawsTextedProgressBar(position, label, currentValue, maxValue, fillColor);
            int newValue = EditorGUI.IntSlider(position, currentValue, 0, maxValue);
            return newValue;
        }

        public static void DrawsTextedProgressBar(string label, float currentValue, float maxValue, Color fillColor, string valueFormat = "0.00", bool showPercentage = false)
        {
            Rect rect = EditorGUILayout.GetControlRect(false);
            DrawsTextedProgressBar(rect, label, currentValue, maxValue, fillColor, valueFormat, showPercentage);
        }

        public static void DrawsTextedProgressBar(Rect position, string label, float currentValue, float maxValue, Color fillColor, string valueFormat = "0.00", bool showPercentage = false)
        {
            DrawProgressBar(position, currentValue, maxValue, fillColor);
            string barText = $"{label} [{currentValue.ToString(valueFormat)}/{maxValue.ToString(valueFormat)}]";
            if (showPercentage)
            {
                float percentage = ((float)currentValue / maxValue) * 100;
                barText += $"({percentage:0}%)";
            }

            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.normal.textColor = Color.white;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;

            style.normal.textColor = Color.black;
            Rect shadowPosition = new Rect(position.x + 2, position.y + 2, position.width, position.height);
            EditorGUI.LabelField(shadowPosition, barText, style);

            style.normal.textColor = Color.white;
            EditorGUI.LabelField(position, barText, style);

            GUI.backgroundColor = new Color(1f, 1f, 1f, 0f);
        }

        public static void DrawProgressBar(float currentValue, float maxValue, Color fillColor)
        {
            Rect rect = EditorGUILayout.GetControlRect(false);
            DrawProgressBar(rect, currentValue, maxValue, fillColor);
        }

        public static void DrawProgressBar(Rect position, float currentValue, float maxValue, Color fillColor)
        {
            float progress = Mathf.Clamp01(currentValue / (float)maxValue);

            Rect darkBackgroundRect = new Rect(position.x, position.y, position.width - 50f, position.height);
            EditorGUI.DrawRect(darkBackgroundRect, new Color(0.15f, 0.15f, 0.15f, 1.0f));

            Rect barRect = new Rect(position.x, position.y, darkBackgroundRect.width * progress, position.height);

            Color originalColor = GUI.color;
            GUI.color = fillColor;
            EditorGUI.DrawRect(barRect, fillColor);
            GUI.color = originalColor;
        }
        #endregion

        /// <summary>
        /// Creates a pop-up menu in the Unity Editor for selecting an enum value.
        /// </summary>
        /// <typeparam name="T">Type of enum</typeparam>
        /// <param name="name">The name of the pop-up menu</param>
        /// <param name="tooltip">A tooltip for the pop-up menu</param>
        /// <param name="index">The index of the currently selected enum value</param>
        /// <returns>An integer value that represents the index of the selected enum value</returns>
        public static int DrawTooltipEnumPopup<T>(string label, string tooltip, int index) where T : System.Enum
        {
            GUIContent content = new GUIContent(label, tooltip);
            return (int)(object)EditorGUILayout.EnumPopup(content, (T)(object)index);
        }

        /// <summary>
        /// Creates a toggle in the Unity Editor with a label and tooltip.
        /// </summary>
        /// <param name="name">The name of the toggle</param>
        /// <param name="tooltip">The tooltip for the toggle</param>
        /// <param name="value">The current value of the toggle</param>
        /// <returns>True if the toggle is selected, otherwise false</returns>
        public static bool DrawTooltipTogle(string label, string tooltip, bool value)
        {
            GUIContent content = new GUIContent(label, tooltip);
            return EditorGUILayout.Toggle(content, value);
        }

        /// <summary>
        /// Creates a text field in the Unity Editor with a label and tooltip.
        /// </summary>
        /// <param name="name">The name of the text field</param>
        /// <param name="tooltip">The tooltip for the text field</param>
        /// <param name="value">The current value of the text field</param>
        /// <returns>The updated value of the text field</returns>
        public static string DrawTooltipTextField(string label, string tooltip, string value)
        {
            GUIContent content = new GUIContent(label, tooltip);
            return EditorGUILayout.TextField(content, value);
        }

        /// <summary>
        /// Draw a text fuekd in the Unity Editor with a label, tooltip and placeholder text.
        /// </summary>
        /// <param name="name">The name of the text field</param>
        /// <param name="tooltip">The tooltip for the text field</param>
        /// <param name="placeholder">The placeholder text</param>
        /// <param name="value">The current value of the text field</param>
        /// <returns>The updated value of the text field</returns>
        public static string DrawPlaceHolderTooltipTextField(string label, string tooltip, string placeholder, string value)
        {
            GUIStyle textFieldStyle = new GUIStyle(EditorStyles.textField);
            GUIContent content = new GUIContent(label, tooltip);

            value = EditorGUILayout.TextField(content, value, textFieldStyle);
            if (string.IsNullOrEmpty(value))
            {
                Rect rect = GUILayoutUtility.GetLastRect();
                rect.x += EditorGUIUtility.labelWidth + 4;
                EditorGUI.LabelField(rect, placeholder, new GUIStyle(EditorStyles.label)
                {
                    fontStyle = FontStyle.Italic,
                    normal = { textColor = Color.gray }
                });
            }
            return value;
        }

        /// <summary>
        /// Draw a text fuekd in the Unity Editor with a label ad tooltip with a message area when has no text.
        /// </summary>
        /// <param name="name">The name of the text field</param>
        /// <param name="tooltip">The tooltip for the text field</param>
        /// <param name="message">The message text</param>
        /// <param name="value">The current value of the text field</param>
        /// <returns>The updated value of the text field</returns>
        public static void DrawToolTipTextFieldWithMessage(SerializedProperty property, string label, string tooltip, string message)
        {
            property.stringValue = DrawTooltipTextField(label, tooltip, property.stringValue);
            if (string.IsNullOrEmpty(property.stringValue))
            {
                DrawTextArea(message);
            }
        }

        /// <summary>
        /// Creates an integer slider in the Unity Editor with a label and tooltip.
        /// </summary>
        /// <param name="name">The name of the slider</param>
        /// <param name="tooltip">The tooltip for the slider</param>
        /// <param name="value">The current value of the slider</param>
        /// <param name="minValue">The minimum value of the slider</param>
        /// <param name="maxValue">The maximum value of the slider</param>
        /// <returns>The updated value of the slider</returns
        public static int DrawTooltipIntSlider(string label, string tooltip, int value, int minValue, int maxValue)
        {
            GUIContent content = new GUIContent(label, tooltip);
            return EditorGUILayout.IntSlider(content, value, minValue, maxValue);
        }

        /// <summary>
        /// Displays a read-only text area with rich text formatting in the Unity Editor.
        /// </summary>
        /// <param name="text">The text to display in the text area</param>
        public static void DrawTextArea(string text)
        {
            GUIStyle style = new GUIStyle("HelpBox");
            style.richText = true;
            EditorGUILayout.LabelField(text, style);
        }

        /// <summary>
        /// Displays a read-only text area with rich text formatting and bold text in the Unity Editor.
        /// </summary>
        /// <param name="text">The main text to display in the text area</param>
        /// <param name="boldText">The bold text to display in the text area</param>
        public static void DrawTextAreaWithBold(string text, string boldText)
        {
            GUIStyle style = new GUIStyle("HelpBox");
            style.richText = true;
            EditorGUILayout.LabelField($"{text}\n<b><color=white>{boldText}</color></b>", style);
        }
    }
}