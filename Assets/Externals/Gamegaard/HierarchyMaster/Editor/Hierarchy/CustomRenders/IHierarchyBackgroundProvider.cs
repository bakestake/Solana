using UnityEngine;

namespace Gamegaard.HierarchyMaster.Editor
{
    public interface IHierarchyBackgroundProvider
    {
        bool ProvidesBackground(GameObject gameObject);
    }
}