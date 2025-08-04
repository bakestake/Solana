using System.Collections.Generic;
using System.Linq;

namespace Gamegaard.Commons.Editor
{
    public class ObjectGroupingHelper<T> : IObjectEditorHelper<T> where T : UnityEngine.ScriptableObject
    {
        public Dictionary<string, T> FilteredObjects { get; private set; }
        public Dictionary<string, Dictionary<string, T>> GroupedObjects { get; private set; }
        public Dictionary<string, T> TrackedObjects { get; private set; }

        public void Initialize(Dictionary<string, T> trackedObjects)
        {
            this.TrackedObjects = new Dictionary<string, T>(trackedObjects);
            FilteredObjects = new Dictionary<string, T>(trackedObjects);
            InitializeGroupedObjects();
        }

        public void ApplySearchFilter(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                FilteredObjects = new Dictionary<string, T>(TrackedObjects);
                InitializeGroupedObjects();
            }
            else
            {
                FilteredObjects = TrackedObjects
                    .Where(pair => pair.Key.ToLower().Contains(query.ToLower()))
                    .ToDictionary(pair => pair.Key, pair => pair.Value);

                InitializeGroupedObjects(FilteredObjects);
            }
        }

        private void InitializeGroupedObjects(Dictionary<string, T> source = null)
        {
            GroupedObjects = new Dictionary<string, Dictionary<string, T>>();

            foreach (var pair in source ?? TrackedObjects)
            {
                string category = GetCategory(pair.Value);

                if (!GroupedObjects.ContainsKey(category))
                {
                    GroupedObjects[category] = new Dictionary<string, T>();
                }

                GroupedObjects[category][pair.Key] = pair.Value;
            }
        }

        public void ReplaceObject(string key, T updatedObject)
        {
            if (TrackedObjects.ContainsKey(key))
            {
                TrackedObjects[key] = updatedObject;
                ApplySearchFilter("");
            }
        }

        public void AddObject(string key, T newObject)
        {
            if (!TrackedObjects.ContainsKey(key))
            {
                TrackedObjects[key] = newObject;
                ApplySearchFilter("");
            }
        }
        protected virtual string GetCategory(T obj)
        {
            return "Default";
        }
    }
}
