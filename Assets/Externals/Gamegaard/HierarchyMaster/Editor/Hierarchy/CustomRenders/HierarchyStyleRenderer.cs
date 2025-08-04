using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Gamegaard.HierarchyMaster.Editor
{
    public class HierarchyStyleRenderer : HierarchyRendererBase, IHierarchyIconProvider, IHierarchyBackgroundProvider
    {
        public int Priority => 0;
        public bool UseBackground => false;

        public override void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (gameObject == null || !gameObject.TryGetComponent(out HierarchyStyle style) || !style.enabled) return;

            HierarchyItem item = new HierarchyItem(instanceID, selectionRect, style);

            PaintBackground(item);
            PaintHoverOverlay(item);
            PaintText(item);
            PaintCollapseToggleIcon(item);
            PaintContainerLines(item);
        }

        public bool ProvidesBackground(GameObject gameObject)
        {
            return gameObject.TryGetComponent(out HierarchyStyle style) && style.BackgroundStyle != BackgroundStyle.Default;
        }

        public GUIContent GetHierarchyIcon(GameObject gameObject)
        {
            if (!gameObject.TryGetComponent(out HierarchyStyle style))
            {
                return null;
            }

            if (style.enabled && style.UseCustomIcon)
            {
                return new GUIContent(style.CustomIcon);
            }

            if (PrefabUtility.GetPrefabAssetType(gameObject) != PrefabAssetType.NotAPrefab)
            {
                return GetCorrectPrefabIcon(gameObject);
            }

            return GetCorrectGameObjectIcon(gameObject);
        }

        private GUIContent GetCorrectPrefabIcon(GameObject gameObject)
        {
            GUIContent defaultIcon = EditorGUIUtility.ObjectContent(gameObject, typeof(GameObject));

            if (UnityThemeUtils.IsHierarchyFocused && Selection.Contains(gameObject))
            {
                if (defaultIcon.image.name == "d_Prefab Icon" || defaultIcon.image.name == "Prefab Icon")
                {
                    return EditorGUIUtility.IconContent("d_Prefab On Icon");
                }
            }

            return defaultIcon;
        }

        private GUIContent GetCorrectGameObjectIcon(GameObject gameObject)
        {
            GUIContent defaultIcon = EditorGUIUtility.ObjectContent(gameObject, typeof(GameObject));

            if (UnityThemeUtils.IsHierarchyFocused && Selection.Contains(gameObject))
            {
                if (defaultIcon.image.name == "GameObject Icon")
                {
                    return EditorGUIUtility.IconContent("GameObject On Icon");
                }
            }

            return defaultIcon;
        }

        private void PaintBackground(HierarchyItem item)
        {
            if (item.Style.BackgroundStyle == BackgroundStyle.Default || item.IsSelectedAndValid)
            {
                Color32 color = UnityThemeUtils.GetDefaultBackgroundColor(UnityThemeUtils.IsHierarchyFocused, item.IsSelectedAndValid, false);
                EditorGUI.DrawRect(item.BackgroundRect, color);
            }
            else if (item.Style.BackgroundStyle == BackgroundStyle.SolidColor)
            {
                EditorGUI.DrawRect(item.BackgroundRect, item.Style.BackgroundColor);
            }
            else if (item.Style.BackgroundStyle == BackgroundStyle.Gradient)
            {
                DrawGradient(item.BackgroundRect, item.Style);
            }
        }

        private void PaintHoverOverlay(HierarchyItem item)
        {
            if (item.IsHovered && !item.IsSelectedAndValid)
            {
                EditorGUI.DrawRect(item.BackgroundRect, UnityThemeUtils.HoverOverlay);
            }
        }

        private void PaintText(HierarchyItem item)
        {
            Color32 color = item.Style.FontColorStyle == FontColorStyle.Default || item.IsSelectedAndValid
                ? UnityThemeUtils.GetDefaultTextColor(UnityThemeUtils.IsHierarchyFocused, item.IsSelectedAndValid, item.GameObject.activeInHierarchy, item.PrefabType != PrefabAssetType.NotAPrefab)
                : item.Style.TextColor;

            GUIStyle labelGUIStyle = new GUIStyle
            {
                normal = new GUIStyleState { textColor = color },
                fontStyle = item.Style.FontStyle,
                alignment = item.Style.Alignment,
                fontSize = item.Style.FontSize,
                font = item.Style.Font,
                richText = item.Style.EnableRichText
            };

            if (item.Style.EnableTextOutline)
            {
                DrawTextOutline(item.TextRect, item.GameObject.name, labelGUIStyle, item.Style.TextOutlineColor);
            }
            else if (item.Style.TextDropShadow)
            {
                EditorGUI.DropShadowLabel(item.TextRect, item.GameObject.name, labelGUIStyle);
            }
            else
            {
                EditorGUI.LabelField(item.TextRect, item.GameObject.name, labelGUIStyle);
            }
        }

        private void PaintCollapseToggleIcon(HierarchyItem item)
        {
            if (item.GameObject.transform.childCount > 0)
            {
                Type sceneHierarchyWindowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
                PropertyInfo sceneHierarchyWindow = sceneHierarchyWindowType.GetProperty("lastInteractedHierarchyWindow", BindingFlags.Public | BindingFlags.Static);

                int[] expandedIDs = (int[])sceneHierarchyWindowType.GetMethod("GetExpandedIDs", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(sceneHierarchyWindow.GetValue(null), null);
                string iconID = expandedIDs.Contains(item.InstanceID) ? "IN Foldout on" : "IN foldout";

                GUI.DrawTexture(item.CollapseToggleIconRect, EditorGUIUtility.IconContent(iconID).image, ScaleMode.StretchToFill, true, 0f, UnityThemeUtils.CollapseIconTintColor, 0f, 0f);
            }
        }

        private void PaintContainerLines(HierarchyItem item)
        {
            if (item.Style.ContainerLines == ContainerLines.None) return;

            Color lineColor = item.Style.ContainerLineColor;
            float thickness = 1.5f;
            Rect rect = item.BackgroundRect;

            if (item.Style.ContainerLines == ContainerLines.Line)
            {
                DrawLine(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y), lineColor, thickness);
                DrawLine(new Vector2(rect.x, rect.y + rect.height), new Vector2(rect.x + rect.width, rect.y + rect.height), lineColor, thickness);
            }
            else if (item.Style.ContainerLines == ContainerLines.Dotted)
            {
                DrawDottedLine(new Vector2(rect.x, rect.y), new Vector2(rect.x + rect.width, rect.y), lineColor);
                DrawDottedLine(new Vector2(rect.x, rect.y + rect.height), new Vector2(rect.x + rect.width, rect.y + rect.height), lineColor);
            }
        }

        private void DrawGradient(Rect rect, HierarchyStyle style)
        {
            if (Event.current.type != EventType.Repaint) return;

            int width = Mathf.Max(1, (int)rect.width);
            int height = Mathf.Max(1, (int)rect.height);
            Texture2D gradientTexture = new Texture2D(width, height);
            gradientTexture.wrapMode = TextureWrapMode.Clamp;

            float radAngle = style.GradientAngle * Mathf.Deg2Rad;
            float cosA = Mathf.Cos(radAngle);
            float sinA = Mathf.Sin(radAngle);

            float maxDistance = Mathf.Abs(width * cosA) + Mathf.Abs(height * sinA);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float normalizedX = (x / (float)(width - 1)) - 0.5f;
                    float normalizedY = (y / (float)(height - 1)) - 0.5f;

                    float projection = normalizedX * cosA + normalizedY * sinA;
                    float t = Mathf.InverseLerp(-0.5f, 0.5f, projection);

                    Color color = style.BackgroundGradient.Evaluate(t);

                    gradientTexture.SetPixel(x, y, color);
                }
            }

            gradientTexture.Apply();
            GUI.DrawTexture(rect, gradientTexture, ScaleMode.StretchToFill);
        }

        private void DrawLine(Vector2 start, Vector2 end, Color color, float thickness)
        {
            Handles.color = color;
            Handles.DrawLine(start, end);
        }

        private void DrawDottedLine(Vector2 start, Vector2 end, Color color)
        {
            Handles.color = color;
            Handles.DrawDottedLine(start, end, 3f);
        }

        private void DrawTextOutline(Rect rect, string text, GUIStyle style, Color outlineColor)
        {
            GUIStyle outlineStyle = new GUIStyle(style)
            {
                normal = { textColor = outlineColor }
            };

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    Rect offsetRect = new Rect(rect.x + x, rect.y + y, rect.width, rect.height);
                    EditorGUI.LabelField(offsetRect, text, outlineStyle);
                }
            }

            EditorGUI.LabelField(rect, text, style);
        }
    }
}