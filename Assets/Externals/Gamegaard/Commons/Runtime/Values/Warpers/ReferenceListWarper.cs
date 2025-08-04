using System;
using System.Collections;
using System.Collections.Generic;

namespace Gamegaard.CustomValues
{
    [Serializable]
    public class ReferenceListWarper<T> : ReferenceValueWarper<List<T>>, ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IList
    {
        object IList.this[int index]
        {
            get => Value[index];
            set
            {
                if (value is T typedValue)
                {
                    Value[index] = typedValue;
                }
                else
                {
                    throw new ArgumentException($"Value must be of type {typeof(T)}.");
                }
            }
        }

        public int Count => Value.Count;
        public bool IsReadOnly => ((ICollection<T>)Value).IsReadOnly;
        public bool IsSynchronized => false;
        public object SyncRoot => this;
        public bool IsFixedSize => false;

        public T this[int index]
        {
            get => Value[index];
            set => Value[index] = value;
        }

        public ReferenceListWarper()
        {
        }

        public ReferenceListWarper(List<T> value) : base(value)
        {
        }

        public void Add(T item) => Value.Add(item);

        public int Add(object value)
        {
            if (value is T typedValue)
            {
                Value.Add(typedValue);
                return Value.Count - 1;
            }
            throw new ArgumentException($"Value must be of type {typeof(T)}.");
        }

        public void Clear() => Value.Clear();

        public bool Contains(T item) => Value.Contains(item);

        public bool Contains(object value) => value is T typedValue && Value.Contains(typedValue);

        public void CopyTo(T[] array, int arrayIndex) => Value.CopyTo(array, arrayIndex);

        public void CopyTo(Array array, int index)
        {
            if (array is T[] typedArray)
            {
                Value.CopyTo(typedArray, index);
            }
            else
            {
                throw new ArgumentException($"Array must be of type {typeof(T[])}.");
            }
        }

        public IEnumerator<T> GetEnumerator() => Value.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int IndexOf(T item) => Value.IndexOf(item);

        public int IndexOf(object value) => value is T typedValue ? Value.IndexOf(typedValue) : -1;

        public void Insert(int index, T item) => Value.Insert(index, item);

        public void Insert(int index, object value)
        {
            if (value is T typedValue)
            {
                Value.Insert(index, typedValue);
            }
            else
            {
                throw new ArgumentException($"Value must be of type {typeof(T)}.");
            }
        }

        public bool Remove(T item) => Value.Remove(item);

        public void Remove(object value)
        {
            if (value is T typedValue)
            {
                Value.Remove(typedValue);
            }
        }

        public void RemoveAt(int index) => Value.RemoveAt(index);
    }
}