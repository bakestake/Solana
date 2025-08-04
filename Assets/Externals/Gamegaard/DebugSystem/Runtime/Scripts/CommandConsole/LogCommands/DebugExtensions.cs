using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

public static class DebugExtensions
{
    public static void DebugCollection<T>(this IEnumerable<T> collection, bool detailed = false)
    {
        if (collection == null)
        {
            Debug.Log("Collection is null.");
            return;
        }

        int count = collection.Count();
        if (count == 0)
        {
            Debug.Log("Collection is empty.");
            return;
        }

        if (detailed)
        {
            string result = string.Join("\n - ", collection.Select(item => item.ToString()));
            Debug.Log($"Collection has {count} items:\n - {result}");
        }
        else
        {
            Debug.Log($"Collection has {count} items.");
        }
    }


    public static void DebugCollection<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, bool detailed = false)
    {
        if (dictionary == null)
        {
            Debug.Log("Collection is null.");
            return;
        }

        int count = dictionary.Count;
        if (count == 0)
        {
            Debug.Log("Collection is empty.");
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.Append($"Dictionary has {count} entries.");
        if (detailed)
        {
            foreach (var pair in dictionary)
            {
                sb.Append($"\nKey: {pair.Key}, Value: {pair.Value}");
            }
        }

        Debug.Log(sb.ToString());
    }
}