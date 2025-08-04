using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard.CustomValues
{
    [Serializable]
    public class SerializableList<T> : ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IList, ISerializationCallbackReceiver where T : class
    {
        [SerializeField] private List<SerializableValue<T>> items = new List<SerializableValue<T>>();
        private List<T> runtimeList;

        public List<SerializableValue<T>> Items => items;

        private bool isDirty = true;

        public int Count
        {
            get
            {
                EnsureRuntimeList();
                return runtimeList.Count;
            }
        }

        public bool IsReadOnly => false;
        public bool IsSynchronized => false;
        public object SyncRoot => ((ICollection)runtimeList).SyncRoot;
        public bool IsFixedSize => false;

        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T)value;
        }

        public T this[int index]
        {
            get
            {
                EnsureRuntimeList();
                return runtimeList[index];
            }
            set
            {
                EnsureRuntimeList();
                runtimeList[index] = value;
                items[index] = new SerializableValue<T>(value);
            }
        }

        public void Add(T item)
        {
            EnsureRuntimeList();
            runtimeList.Add(item);
            items.Add(new SerializableValue<T>(item));
        }

        public bool Remove(T item)
        {
            EnsureRuntimeList();
            int index = runtimeList.IndexOf(item);
            if (index >= 0)
            {
                runtimeList.RemoveAt(index);
                items.RemoveAt(index);
                return true;
            }
            return false;
        }

        public void Clear()
        {
            EnsureRuntimeList();
            runtimeList.Clear();
            items.Clear();
        }

        public bool Contains(T item)
        {
            EnsureRuntimeList();
            return runtimeList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            EnsureRuntimeList();
            runtimeList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            EnsureRuntimeList();
            return runtimeList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int IndexOf(T item)
        {
            EnsureRuntimeList();
            return runtimeList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            EnsureRuntimeList();
            runtimeList.Insert(index, item);
            items.Insert(index, new SerializableValue<T>(item));
        }

        public void RemoveAt(int index)
        {
            EnsureRuntimeList();
            runtimeList.RemoveAt(index);
            items.RemoveAt(index);
        }

        public void CopyTo(Array array, int index)
        {
            EnsureRuntimeList();
            ((ICollection)runtimeList).CopyTo(array, index);
        }

        public int Add(object value)
        {
            if (value is T item)
            {
                Add(item);
                return Count - 1;
            }
            throw new ArgumentException("Invalid type");
        }

        public bool Contains(object value) => value is T item && Contains(item);

        public int IndexOf(object value) => value is T item ? IndexOf(item) : -1;

        public void Insert(int index, object value)
        {
            if (value is T item)
            {
                Insert(index, item);
            }
            else
            {
                throw new ArgumentException("Invalid type");
            }
        }

        public void Remove(object value)
        {
            if (value is T item)
            {
                Remove(item);
            }
        }

        public static implicit operator List<T>(SerializableList<T> serializableValue)
        {
            serializableValue.EnsureRuntimeList();
            return serializableValue.runtimeList;
        }

        public static implicit operator SerializableList<T>(List<T> value)
        {
            SerializableList<T> serializableList = new SerializableList<T>();
            foreach (T item in value)
            {
                serializableList.Add(item);
            }
            return serializableList;
        }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            RebuildRuntimeList();
        }

        private void EnsureRuntimeList()
        {
            if (isDirty)
            {
                RebuildRuntimeList();
            }
        }

        public void RebuildRuntimeList()
        {
            runtimeList = new List<T>();
            foreach (SerializableValue<T> item in items)
            {
                runtimeList.Add(item.Value);
            }
        }
    }
}
