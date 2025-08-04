using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

namespace ALF.Piramipris
{
    /// <summary>
    /// Represents a collection of items that supports unique, non-repeating random selection in cycles.
    /// The collection allows for resetting the cycle when all items have been used.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    [Serializable]
    public class UniqueCycleGroup<T> : ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IList
    {
        /// <summary>
        /// The list of all items available for selection.
        /// </summary>
        [SerializeField] private List<T> allItems;

        /// <summary>
        /// Indicates whether the cycle resets automatically when all items have been used.
        /// </summary>
        [SerializeField] private bool isResetedWhenEmpty = true;

        /// <summary>
        /// The list of items that have been selected in the current cycle.
        /// </summary>
        private readonly List<T> usedItems;

        /// <summary>
        /// Gets the list of all items.
        /// </summary>
        public List<T> AllItems => allItems;

        /// <summary>
        /// Gets the list of items that have been used in the current cycle.
        /// </summary>
        public List<T> UsedItems => usedItems;

        /// <summary>
        /// Gets the number of unused items remaining in the current cycle.
        /// </summary>
        public int UnusedCount => allItems.Count - usedItems.Count;

        /// <summary>
        /// Gets or sets whether the cycle resets automatically when all items have been used.
        /// </summary>
        public bool IsResetedWhenEmpty
        {
            get => isResetedWhenEmpty;
            set => isResetedWhenEmpty = value;
        }

        /// <summary>
        /// Gets the total number of items in the collection.
        /// </summary>
        public int Count => allItems.Count;

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets a value indicating whether access to the collection is synchronized (thread-safe).
        /// </summary>
        public bool IsSynchronized => false;

        /// <summary>
        /// Gets an object that can be used to synchronize access to the collection.
        /// </summary>
        public object SyncRoot => null;

        /// <summary>
        /// Gets a value indicating whether the collection has a fixed size.
        /// </summary>
        public bool IsFixedSize => false;

        /// <summary>
        /// Gets or sets the item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to get or set.</param>
        object IList.this[int index]
        {
            get => allItems[index];
            set => allItems[index] = (T)value;
        }

        /// <summary>
        /// Gets or sets the item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to get or set.</param>
        public T this[int index]
        {
            get => allItems[index];
            set => allItems[index] = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueCycleGroup{T}"/> class.
        /// </summary>
        public UniqueCycleGroup()
        {
            allItems = new List<T>();
            usedItems = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueCycleGroup{T}"/> class with the specified items.
        /// </summary>
        /// <param name="allItems">The items to initialize the collection with.</param>
        /// <param name="isResetedWhenEmpty">Whether the cycle resets automatically when all items are used.</param>
        public UniqueCycleGroup(IEnumerable<T> allItems, bool isResetedWhenEmpty = true)
        {
            this.allItems = allItems.ToList();
            this.isResetedWhenEmpty = isResetedWhenEmpty;
            usedItems = new List<T>();
        }

        /// <summary>
        /// Reinitializes the collection with a new set of items.
        /// </summary>
        /// <param name="allItems">The new set of items to use.</param>
        public void Renitialize(IEnumerable<T> allItems)
        {
            this.allItems = allItems.ToList();
            usedItems.Clear();
        }

        /// <summary>
        /// Selects the next unique item from the collection.
        /// Resets the cycle if all items have been used and <see cref="IsResetedWhenEmpty"/> is true.
        /// </summary>
        /// <returns>The next unique item.</returns>
        public T SelectNext()
        {
            T[] valuesExcept = allItems.Except(usedItems).ToArray();
            T selected = valuesExcept.ElementAt(Random.Range(0, valuesExcept.Length));

            if (isResetedWhenEmpty && UnusedCount <= 1)
            {
                usedItems.Clear();
            }
            usedItems.Add(selected);
            return selected;
        }

        /// <summary>
        /// Resets the list of used items, allowing all items to be selected again.
        /// </summary>
        public void ResetUsedItems()
        {
            usedItems.Clear();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator for the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in allItems)
                yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(T item)
        {
            allItems.Add(item);
        }

        /// <summary>
        /// Clears all items from the collection, including the used items.
        /// </summary>
        public void Clear()
        {
            allItems.Clear();
            usedItems.Clear();
        }

        /// <summary>
        /// Determines whether the collection contains a specific item.
        /// </summary>
        /// <param name="item">The item to locate.</param>
        /// <returns>True if the item is found; otherwise, false.</returns>
        public bool Contains(T item)
        {
            return allItems.Contains(item);
        }

        /// <summary>
        /// Copies the elements of the collection to an array, starting at a particular index.
        /// </summary>
        /// <param name="array">The destination array.</param>
        /// <param name="arrayIndex">The index in the array to start copying to.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            allItems.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific item from the collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True if the item was successfully removed; otherwise, false.</returns>
        public bool Remove(T item)
        {
            return allItems.Remove(item);
        }

        /// <summary>
        /// Determines the index of a specific item in the collection.
        /// </summary>
        /// <param name="item">The item to locate.</param>
        /// <returns>The index of the item, or -1 if not found.</returns>
        public int IndexOf(T item)
        {
            return allItems.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index to insert the item at.</param>
        /// <param name="item">The item to insert.</param>
        public void Insert(int index, T item)
        {
            allItems.Insert(index, item);
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            allItems.RemoveAt(index);
        }

        /// <summary>
        /// Copies the elements of the collection to an array, starting at a particular index.
        /// </summary>
        /// <param name="array">The destination array.</param>
        /// <param name="index">The index in the array to start copying to.</param>
        public void CopyTo(Array array, int index)
        {
            ((ICollection)allItems).CopyTo(array, index);
        }

        /// <summary>
        /// Adds an item to the collection and returns its index.
        /// </summary>
        /// <param name="value">The item to add.</param>
        /// <returns>The index of the added item.</returns>
        public int Add(object value)
        {
            allItems.Add((T)value);
            return allItems.Count - 1;
        }

        /// <summary>
        /// Determines whether the collection contains a specific item.
        /// </summary>
        /// <param name="value">The item to locate.</param>
        /// <returns>True if the item is found; otherwise, false.</returns>
        public bool Contains(object value)
        {
            return Contains((T)value);
        }

        /// <summary>
        /// Determines the index of a specific item in the collection.
        /// </summary>
        /// <param name="value">The item to locate.</param>
        /// <returns>The index of the item, or -1 if not found.</returns>
        public int IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index to insert the item at.</param>
        /// <param name="value">The item to insert.</param>
        public void Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        /// <summary>
        /// Removes the first occurrence of a specific item from the collection.
        /// </summary>
        /// <param name="value">The item to remove.</param>
        public void Remove(object value)
        {
            Remove((T)value);
        }
    }
}