using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamegaard.CustomValues
{
    [System.Serializable]
    public class SerializableDictionary<KeyType, ValueType> : Dictionary<KeyType, ValueType>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<SerializableKeyValuePair<KeyType, ValueType>> keyValuePairs = new();

        public void OnBeforeSerialize()
        {
            foreach (KeyValuePair<KeyType, ValueType> kvp in this)
            {
                if (!keyValuePairs.Any(pair => pair.key.Equals(kvp.Key)))
                {
                    keyValuePairs.Add(new SerializableKeyValuePair<KeyType, ValueType>(kvp.Key, kvp.Value));
                }
            }

            keyValuePairs.RemoveAll(pair => !ContainsKey(pair.key));
        }

        public void OnAfterDeserialize()
        {
            SynchDictionaryData();
        }

        private void SynchDictionaryData()
        {
            Clear();

            foreach (SerializableKeyValuePair<KeyType, ValueType> kvp in keyValuePairs)
            {
                KeyType key = kvp.key;
                if (key != null && !ContainsKey(key))
                {
                    this[key] = kvp.value.Value;
                }
            }
        }
    }
}
