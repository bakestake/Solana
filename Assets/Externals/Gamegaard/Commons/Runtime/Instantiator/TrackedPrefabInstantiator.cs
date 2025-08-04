using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace Gamegaard
{
    [System.Serializable]
    public class TrackedPrefabInstantiator<T> : PrefabInstantiator<T> where T : Component
    {
        protected readonly List<T> instantiatedList = new List<T>();
        public readonly ReadOnlyCollection<T> InstantiatedList;
        public int Count => instantiatedList.Count;

        public TrackedPrefabInstantiator()
        {
            InstantiatedList = instantiatedList.AsReadOnly();
        }

        protected override void OnPrefabCreated(T newObject)
        {
            instantiatedList.Add(newObject);
        }

        public void DestroyInstantiatedObjects()
        {
            foreach (T item in instantiatedList)
            {
                Object.Destroy(item.gameObject);
            }
            instantiatedList.Clear();
        }

        public void DestroyInstance(T instance)
        {
            if (instantiatedList.Contains(instance))
            {
                Object.Destroy(instance.gameObject);
                instantiatedList.Remove(instance);
            }
        }

        public void ActivateAllInstances(bool isActive)
        {
            foreach (T instance in instantiatedList)
            {
                instance.gameObject.SetActive(isActive);
            }
        }

        public List<T> GetInstancesByCondition(System.Func<T, bool> condition)
        {
            return instantiatedList.Where(condition).ToList();
        }

        public void RandomizeSiblingIndices()
        {
            List<int> indices = Enumerable.Range(0, Count).OrderBy(x => Random.value).ToList();

            for (int i = 0; i < Count; i++)
            {
                instantiatedList[i].transform.SetSiblingIndex(indices[i]);
            }
        }
    }
}