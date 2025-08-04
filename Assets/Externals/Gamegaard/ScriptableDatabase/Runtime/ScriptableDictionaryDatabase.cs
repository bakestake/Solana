using System.Collections.Generic;
using System.Linq;

namespace Gamegaard.CustomValues.Database
{
    public abstract class ScriptableDictionaryDatabase<TValue> : ScriptableDatabaseBase<TValue>
    {
        protected abstract SerializableDictionary<string, TValue> DictionaryDatas { get; }
        public SerializableDictionary<string, TValue> NamedDatas => DictionaryDatas;
        public sealed override List<TValue> Datas => DictionaryDatas.Values.ToList();

        public virtual bool AddData(string key, TValue data)
        {
            if (data == null || DictionaryDatas.ContainsKey(key)) return false;

            DictionaryDatas.Add(key, data);

            return true;
        }

        public virtual bool TryGetByID(string key, out TValue value)
        {
            return DictionaryDatas.TryGetValue(key, out value);
        }

        public virtual bool ContaisID(string key)
        {
            return DictionaryDatas.ContainsKey(key);
        }

        public virtual bool RemoveData(string key)
        {
            return DictionaryDatas.Remove(key);
        }

        public virtual void Clear()
        {
            Datas.Clear();
        }
    }
}