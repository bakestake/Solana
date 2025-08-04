using UnityEditor;
using UnityEngine;

namespace Gamegaard.Commons.Editor.Utils
{
    public static class GamegaardGUILayouts
    {
        public static void BeginVerticalCheckWindow(string title, bool isCompleted, Texture2D checkTexture, Texture2D crossTexture)
        {
            GUIStyle windowStyle = GUI.skin.window;
            windowStyle.padding.top = 5;
            EditorGUILayout.BeginVertical(windowStyle);

            Texture2D completudeIcon = isCompleted ? checkTexture : crossTexture;
            string completudeText = isCompleted ? "All tasks in this section are complete" : "There are outstanding tasks in this section";

            GUILayout.BeginHorizontal();
            GUIStyle titleLabel = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                richText = true,
                alignment = TextAnchor.MiddleCenter
            };
            GUILayout.Label($"<b>{title}</b>", titleLabel, GUILayout.MaxHeight(25));
            GUILayout.FlexibleSpace();
            GUILayout.Label(new GUIContent(completudeIcon, completudeText), GUILayout.MaxHeight(25));
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
        }

        public static void BeginVerticalWindow(string title)
        {
            GUIStyle windowStyle = GUI.skin.window;
            windowStyle.padding.top = 5;
            EditorGUILayout.BeginVertical(windowStyle);

            GUIStyle titleLabel = new GUIStyle("Label");
            titleLabel.fontSize = 16;
            titleLabel.richText = true;
            GUILayout.Label($"<b>{title}</b>", titleLabel);
        }

        public static void EndVerticalWindow()
        {
            EditorGUILayout.EndVertical();
            GUILayout.Space(15);
        }
    }
}