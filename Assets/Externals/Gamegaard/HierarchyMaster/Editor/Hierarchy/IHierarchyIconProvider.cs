using UnityEngine;

namespace Gamegaard.HierarchyMaster.Editor
{
    public interface IHierarchyIconProvider
    {
        int Priority { get; }
        bool UseBackground {  get; }
        GUIContent GetHierarchyIcon(GameObject gameObject);
    }
}