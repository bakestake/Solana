using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard.Commons.Editor
{
    public interface IObjectEditorHelper<T> where T : ScriptableObject
    {
        Dictionary<string, Dictionary<string, T>> GroupedObjects { get; }
        Dictionary<string, T> FilteredObjects { get; }
        Dictionary<string, T> TrackedObjects { get; }
        void Initialize(Dictionary<string, T> trackedObjects);
        void ApplySearchFilter(string searchQuery);
        void ReplaceObject(string key, T updatedObject);
        void AddObject(string key, T newObject);
    }
}
