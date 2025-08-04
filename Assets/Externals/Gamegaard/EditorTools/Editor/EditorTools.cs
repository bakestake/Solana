using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Gamegaard.EditorTools
{
    public static class EditorTools
    {
        [MenuItem("CONTEXT/Button/Rename Text")]
        private static void RenameButtonText(MenuCommand menuCommand)
        {
            Button selectedObject = menuCommand.context as Button;

            if (selectedObject != null)
            {
                TextMeshProUGUI tmp = selectedObject.GetComponentInChildren<TextMeshProUGUI>();
                if (tmp != null)
                {
                    Undo.RecordObject(tmp, "Rename Button Text");
                    tmp.name = $"{selectedObject.name}Text";
                    return;
                }

                Text text = selectedObject.GetComponentInChildren<Text>();
                if (text != null)
                {
                    Undo.RecordObject(tmp, "Rename Button Text");
                    text.name = $"{selectedObject.name}Text";
                }

                Debug.LogWarning($"No Valid text component found in [{selectedObject.name}]");
            }
        }

        [MenuItem("CONTEXT/Button/Rename Text", true)]
        private static bool ValidateRenameButtonText(MenuCommand menuCommand)
        {
            Button selectedObject = menuCommand.context as Button;
            return selectedObject != null && selectedObject.GetComponentInChildren<TextMeshProUGUI>() != null;
        }
    }

    public static class TextConverterTools
    {
        [MenuItem("CONTEXT/Text/Convert to TextMeshProUGUI")]
        private static void ConvertTextToTextMeshProUGUI(MenuCommand menuCommand)
        {
            Text text = menuCommand.context as Text;

            if (text != null)
            {
                Undo.RecordObject(text.gameObject, "Convert Text to TextMeshProUGUI");

                GameObject textGO = text.gameObject;
                string originalText = text.text;
                FontStyle originalStyle = text.fontStyle;
                int originalFontSize = text.fontSize;
                Color originalColor = text.color;
                TextAnchor originalAlignment = text.alignment;
                bool originalRaycastTarget = text.raycastTarget;
                bool originalMaskable = text.maskable;

                Object.DestroyImmediate(text);

                TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
                tmp.text = originalText;
                tmp.fontSize = originalFontSize;
                tmp.fontStyle = ConvertFontStyle(originalStyle);
                tmp.color = originalColor;
                tmp.alignment = ConvertTextAnchor(originalAlignment);
                tmp.raycastTarget = originalRaycastTarget;
                tmp.maskable = originalMaskable;
                tmp.enableWordWrapping = true;
                tmp.richText = true;

                Debug.Log($"Converted Text to TextMeshProUGUI in [{textGO.name}]");
            }
        }

        [MenuItem("CONTEXT/TextMeshProUGUI/Convert to Text")]
        private static void ConvertTextMeshProUGUIToText(MenuCommand menuCommand)
        {
            TextMeshProUGUI tmp = menuCommand.context as TextMeshProUGUI;

            if (tmp != null)
            {
                Undo.RecordObject(tmp.gameObject, "Convert TextMeshProUGUI to Text");

                GameObject textGO = tmp.gameObject;
                string originalText = tmp.text;
                int originalFontSize = Mathf.RoundToInt(tmp.fontSize);
                Color originalColor = tmp.color;
                TextAnchor originalAlignment = ConvertTMPAlignment(tmp.alignment);
                bool originalRaycastTarget = tmp.raycastTarget;
                bool originalMaskable = tmp.maskable;

                Object.DestroyImmediate(tmp);

                Text text = textGO.AddComponent<Text>();
                text.text = originalText;
                text.fontSize = originalFontSize;
                text.color = originalColor;
                text.alignment = originalAlignment;
                text.raycastTarget = originalRaycastTarget;
                text.maskable = originalMaskable;

                Debug.Log($"Converted TextMeshProUGUI to Text in [{textGO.name}]");
            }
        }

        [MenuItem("CONTEXT/Text/Convert to TextMeshProUGUI", true)]
        private static bool ValidateConvertTextToTextMeshProUGUI(MenuCommand menuCommand)
        {
            return menuCommand.context is Text;
        }

        [MenuItem("CONTEXT/TextMeshProUGUI/Convert to Text", true)]
        private static bool ValidateConvertTextMeshProUGUIToText(MenuCommand menuCommand)
        {
            return menuCommand.context is TextMeshProUGUI;
        }

        private static FontStyles ConvertFontStyle(FontStyle fontStyle)
        {
            return fontStyle switch
            {
                FontStyle.Bold => FontStyles.Bold,
                FontStyle.Italic => FontStyles.Italic,
                FontStyle.BoldAndItalic => FontStyles.Bold | FontStyles.Italic,
                _ => FontStyles.Normal,
            };
        }

        private static TextAlignmentOptions ConvertTextAnchor(TextAnchor anchor)
        {
            return anchor switch
            {
                TextAnchor.UpperLeft => TextAlignmentOptions.TopLeft,
                TextAnchor.UpperCenter => TextAlignmentOptions.Top,
                TextAnchor.UpperRight => TextAlignmentOptions.TopRight,
                TextAnchor.MiddleLeft => TextAlignmentOptions.MidlineLeft,
                TextAnchor.MiddleCenter => TextAlignmentOptions.Center,
                TextAnchor.MiddleRight => TextAlignmentOptions.MidlineRight,
                TextAnchor.LowerLeft => TextAlignmentOptions.BottomLeft,
                TextAnchor.LowerCenter => TextAlignmentOptions.Bottom,
                TextAnchor.LowerRight => TextAlignmentOptions.BottomRight,
                _ => TextAlignmentOptions.Center,
            };
        }

        private static TextAnchor ConvertTMPAlignment(TextAlignmentOptions alignment)
        {
            return alignment switch
            {
                TextAlignmentOptions.TopLeft => TextAnchor.UpperLeft,
                TextAlignmentOptions.Top => TextAnchor.UpperCenter,
                TextAlignmentOptions.TopRight => TextAnchor.UpperRight,
                TextAlignmentOptions.MidlineLeft => TextAnchor.MiddleLeft,
                TextAlignmentOptions.Center => TextAnchor.MiddleCenter,
                TextAlignmentOptions.MidlineRight => TextAnchor.MiddleRight,
                TextAlignmentOptions.BottomLeft => TextAnchor.LowerLeft,
                TextAlignmentOptions.Bottom => TextAnchor.LowerCenter,
                TextAlignmentOptions.BottomRight => TextAnchor.LowerRight,
                _ => TextAnchor.MiddleCenter,
            };
        }
    }
}