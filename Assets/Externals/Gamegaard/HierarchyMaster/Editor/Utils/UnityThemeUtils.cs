using UnityEditor;
using UnityEngine;

namespace Gamegaard.HierarchyMaster.Editor
{
    public static class UnityThemeUtils
    {
        // DARK THEME - Background
        public readonly static Color32 darkBackground = new Color32(56, 56, 56, 255);
        public readonly static Color32 darkObjectSelectedBackground = new Color32(77, 77, 77, 255);
        public readonly static Color32 darkObjectSelectedWindowFocusedBackground = new Color32(44, 93, 134, 255);
        public readonly static Color32 darkOverOverlay = new Color32(255, 255, 255, 15);
        public readonly static Color32 opaqueDarkOverOverlay = new Color32(68, 68, 68, 255);

        // DARK THEME - Text
        public readonly static Color32 darkText = new Color32(210, 210, 210, 255);
        public readonly static Color32 darkTextHighlighted = new Color32(255, 255, 255, 255);
        public readonly static Color32 darkPrefabText = new Color32(120, 174, 234, 255);
        public const byte darkTextAlphaObjectEnabled = 255;
        public const byte darkTextAlphaObjectDisabled = 103;

        // LIGHT THEME - Background
        public readonly static Color32 lightBackground = new Color32(200, 200, 200, 255);
        public readonly static Color32 lightObjectSelectedBackground = new Color32(178, 178, 178, 255);
        public readonly static Color32 lightObjectSelectedWindowFocusedBackground = new Color32(58, 114, 176, 255);
        public readonly static Color32 lightHoverOverlay = new Color32(0, 0, 0, 21);
        public readonly static Color32 opaqueLightHoverOverlay = new Color32(178, 178, 178, 255);

        // LIGHT THEME - Text
        public readonly static Color32 lightText = new Color32(2, 2, 2, 255);
        public readonly static Color32 lightTextHighlighted = new Color32(255, 255, 255, 255);
        public readonly static Color32 lightPrefabText = new Color32(135, 206, 250, 255);
        public const byte lightTextAlphaObjectEnabled = 255;
        public const byte lightTextAlphaObjectDisabled = 95;

        public static Color32 Background => EditorGUIUtility.isProSkin ? darkBackground : lightBackground;
        public static Color32 ObjectSelectedBackground => EditorGUIUtility.isProSkin ? darkObjectSelectedBackground : lightObjectSelectedBackground;
        public static Color32 ObjectSelectedWindowFocusedBackground => EditorGUIUtility.isProSkin ? darkObjectSelectedWindowFocusedBackground : lightObjectSelectedWindowFocusedBackground;
        public static Color32 HoverOverlay => EditorGUIUtility.isProSkin ? darkOverOverlay : lightHoverOverlay;
        public static Color32 OpaqueHoverOverlay => EditorGUIUtility.isProSkin ? opaqueDarkOverOverlay : opaqueLightHoverOverlay;
        public static Color32 Text => EditorGUIUtility.isProSkin ? darkText : lightText;
        public static Color32 TextHighlighted => EditorGUIUtility.isProSkin ? darkTextHighlighted : lightTextHighlighted;
        public static byte TextAlphaObjectEnabled => EditorGUIUtility.isProSkin ? darkTextAlphaObjectEnabled : lightTextAlphaObjectEnabled;
        public static byte TextAlphaObjectDisabled => EditorGUIUtility.isProSkin ? darkTextAlphaObjectDisabled : lightTextAlphaObjectDisabled;
        public static Color CollapseIconTintColor => EditorGUIUtility.isProSkin ? Color.white : Color.black;
        public static Color EditPrefabIconTintColor => EditorGUIUtility.isProSkin ? Color.white : Color.black;
        public static bool IsHierarchyFocused => EditorWindow.focusedWindow != null && EditorWindow.focusedWindow.titleContent.text == "Hierarchy";

        public static Color32 GetDefaultBackgroundColor(bool windowIsFocused, bool isSelected, bool isHovered)
        {
            if (isSelected)
                return windowIsFocused ? ObjectSelectedWindowFocusedBackground : ObjectSelectedBackground;

            if (isHovered)
                return OpaqueHoverOverlay;

            return Background;
        }

        public static Color32 GetDefaultTextColor(bool windowIsFocused, bool selectionContainsObject, bool objectIsEnabled, bool isPrefab)
        {
            bool textHighlighted = IsTextHighlighted(windowIsFocused, selectionContainsObject, objectIsEnabled);

            Color32 color;

            if (EditorGUIUtility.isProSkin)
            {
                color = isPrefab ? darkPrefabText : darkText;
                if (textHighlighted)
                {
                    color = darkTextHighlighted;
                }
                color.a = objectIsEnabled ? darkTextAlphaObjectEnabled : darkTextAlphaObjectDisabled;
            }
            else
            {
                color = isPrefab ? lightPrefabText : lightText;
                if (textHighlighted)
                {
                    color = lightTextHighlighted;
                }
                color.a = objectIsEnabled ? lightTextAlphaObjectEnabled : lightTextAlphaObjectDisabled;
            }

            return color;
        }


        public static bool IsTextHighlighted(bool windowIsFocused, bool selectionContainsObject, bool objectIsEnabled)
        {
            return windowIsFocused && selectionContainsObject && objectIsEnabled;
        }
    }
}