using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TabMenu))]
public class TabMenuEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("GenerateTabs"))
        {
            TabMenu tabMenu = (TabMenu)target;
            tabMenu.GenerateTabs();
        }
    }
}
