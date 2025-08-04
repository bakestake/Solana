using UnityEngine;

namespace Gamegaard.HierarchyMaster.Editor
{
    public abstract class HierarchyRendererBase
    {
        public abstract void OnHierarchyGUI(int instanceID, Rect selectionRect);
    }
}