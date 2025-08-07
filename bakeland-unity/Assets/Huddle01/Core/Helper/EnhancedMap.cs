using System;
using System.Collections.Generic;

namespace Huddle01.Helper
{
    public class EnhancedMap<T>
    {
        private Dictionary<string, T> _map;
        private Func<string, string, bool> _compareFn;

        public EnhancedMap(Func<string, string, bool>? compareFn = null)
        {
            _map = new Dictionary<string, T>();
            _compareFn = compareFn ?? DefaultCompareFn;
        }

        // Returns the number of key-value pairs in the map
        public int Size => _map.Count;

        // Retrieves a value using two strings as key components
        public T? Get(string a, string b)
        {
            var key = GetKey(a, b);
            _map.TryGetValue(key, out var value);
            return value;
        }

        // Sets a value in the map using two strings as key components
        public T Set(string a, string b, T value)
        {
            var key = GetKey(a, b);
            _map[key] = value;
            return value;
        }

        // Deletes a key-value pair from the map using two strings as key components
        public bool Delete(string a, string b)
        {
            var key = GetKey(a, b);
            return _map.Remove(key);
        }

        // Clears all key-value pairs from the map
        public void Clear()
        {
            _map.Clear();
        }

        // Generates a unique key based on the comparison function
        private string GetKey(string a, string b)
        {
            return _compareFn(a, b) ? $"{a}_{b}" : $"{b}_{a}";
        }

        // Default comparison function compares strings lexicographically
        private static bool DefaultCompareFn(string a, string b)
        {
            return string.Compare(a, b) > 0;
        }
    }
}
