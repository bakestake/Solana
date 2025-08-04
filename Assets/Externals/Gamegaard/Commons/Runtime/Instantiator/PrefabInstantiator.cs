using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Gamegaard
{
    [Serializable]
    public class PrefabInstantiator<T> where T : Object
    {
        [SerializeField] private T prefab;
        [SerializeField] private Transform parent;

        public T Prefab => prefab;
        public Transform Parent => parent;

        public virtual void SetPrefab(T prefab)
        {
            this.prefab = prefab;
        }

        public virtual void SetParent(Transform parent)
        {
            this.parent = parent;
        }

        public virtual T Instantiate()
        {
            T newObject = Object.Instantiate(prefab, parent);
            OnPrefabCreated(newObject);
            return newObject;
        }

        public virtual T Instantiate(Vector3 position)
        {
            return Instantiate(position, Quaternion.identity);
        }

        public virtual T Instantiate(Vector3 position, Quaternion quaternion)
        {
            T newObject = Object.Instantiate(prefab, position, quaternion, parent);
            OnPrefabCreated(newObject);
            return newObject;
        }

        public virtual T Instantiate(Action<T> action)
        {
            T newObject = Object.Instantiate(prefab, parent);
            action.Invoke(newObject);
            OnPrefabCreated(newObject);
            return newObject;
        }

        public virtual T Instantiate(Vector3 position, Action<T> action)
        {
            return Instantiate(position, Quaternion.identity, action);
        }

        public virtual T Instantiate(Vector3 position, Quaternion quaternion, Action<T> action)
        {
            T newObject = Object.Instantiate(prefab, position, quaternion, parent);
            action.Invoke(newObject);
            OnPrefabCreated(newObject);
            return newObject;
        }

        public virtual List<T> Instantiate(int count)
        {
            List<T> instances = new List<T>();
            for (int i = 0; i < count; i++)
            {
                T newObject = Object.Instantiate(prefab, parent);
                OnPrefabCreated(newObject);
                instances.Add(newObject);
            }
            return instances;
        }

        public virtual List<T> Instantiate(int count, Action<T> action)
        {
            List<T> instances = new List<T>();
            for (int i = 0; i < count; i++)
            {
                T newObject = Object.Instantiate(prefab, parent);
                action.Invoke(newObject);
                OnPrefabCreated(newObject);
                instances.Add(newObject);
            }
            return instances;
        }

        public virtual List<T> Instantiate(int count, Vector3 position)
        {
            return Instantiate(count, position, Quaternion.identity);
        }

        public virtual List<T> Instantiate(int count, Vector3 position, Quaternion quaternion)
        {
            List<T> instances = new List<T>();
            for (int i = 0; i < count; i++)
            {
                T newObject = Object.Instantiate(prefab, position, quaternion, parent);
                OnPrefabCreated(newObject);
                instances.Add(newObject);
            }
            return instances;
        }

        public virtual List<T> Instantiate(int count, Vector3 position, Action<T> action)
        {
            return Instantiate(count, position, Quaternion.identity, action);
        }

        public virtual List<T> Instantiate(int count, Vector3 position, Quaternion quaternion, Action<T> action)
        {
            List<T> instances = new List<T>();
            for (int i = 0; i < count; i++)
            {
                T newObject = Object.Instantiate(prefab, position, quaternion, parent);
                action.Invoke(newObject);
                OnPrefabCreated(newObject);
                instances.Add(newObject);
            }
            return instances;
        }

        protected virtual void OnPrefabCreated(T newObject)
        {
        }
    }
}