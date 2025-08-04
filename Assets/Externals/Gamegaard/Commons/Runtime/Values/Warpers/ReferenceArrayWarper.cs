using System;
using System.Collections;
using System.Collections.Generic;

namespace Gamegaard.CustomValues
{
    [Serializable]
    public class ReferenceArrayWarper<T> : ReferenceValueWarper<T[]>, ICollection, IEnumerable, IList, IStructuralComparable, IStructuralEquatable, ICloneable
    {
        public object this[int index]
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

        public bool IsFixedSize => true;
        public bool IsReadOnly => false;
        public int Count => Value.Length;
        public bool IsSynchronized => false;
        public object SyncRoot => this;

        public ReferenceArrayWarper()
        {
        }

        public ReferenceArrayWarper(T[] value) : base(value)
        {
        }

        public int CompareTo(object other, IComparer comparer)
        {
            if (other is T[] otherArray)
            {
                return StructuralComparisons.StructuralComparer.Compare(Value, otherArray);
            }
            throw new ArgumentException("Object is not of the correct type.");
        }

        public bool Equals(object other, IEqualityComparer comparer)
        {
            if (other is T[] otherArray)
            {
                return StructuralComparisons.StructuralEqualityComparer.Equals(Value, otherArray);
            }
            return false;
        }

        public int Add(object value) => throw new NotSupportedException("Cannot add to a fixed-size array.");
        public void Clear() => Array.Clear(Value, 0, Value.Length);
        public object Clone() => Value.Clone();
        public bool Contains(object value) => value is T typedValue && Array.Exists(Value, element => EqualityComparer<T>.Default.Equals(element, typedValue));
        public void CopyTo(Array array, int index) => Array.Copy(Value, 0, array, index, Value.Length);
        public IEnumerator GetEnumerator() => Value.GetEnumerator();
        public int GetHashCode(IEqualityComparer comparer) => StructuralComparisons.StructuralEqualityComparer.GetHashCode(Value);
        public int IndexOf(object value) => value is T typedValue ? Array.IndexOf(Value, typedValue) : -1;
        public void Insert(int index, object value) => throw new NotSupportedException("Cannot insert into a fixed-size array.");
        public void Remove(object value) => throw new NotSupportedException("Cannot remove from a fixed-size array.");
        public void RemoveAt(int index) => throw new NotSupportedException("Cannot remove from a fixed-size array.");
    }
}