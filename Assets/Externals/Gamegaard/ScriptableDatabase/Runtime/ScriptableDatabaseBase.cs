using Gamegaard.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gamegaard.CustomValues.Database
{
    public abstract class ScriptableDatabaseBase<TValue> : ScriptableObject
    {
        public abstract List<TValue> Datas { get; }
        public int Count => Datas.Count;
        public bool HasAnyData => Datas.Count > 0;

        public virtual bool ContaisData(TValue data)
        {
            return Datas.Contains(data);
        }

        public TValue GetRandom()
        {
            return Datas.GetRandom();
        }

        public List<TValue> GetRandomAmount(int amount)
        {
            return Datas.GetRandomAmount(amount);
        }

        public List<TValue> GetRandomIntersect(int amount, IEnumerable<TValue> dataValues)
        {
            return Datas.GetRandomIntersect(amount, dataValues);
        }

        public List<TValue> GetRandomExcept(int amount, IEnumerable<TValue> dataValues)
        {
            return Datas.GetRandomExcept(amount, dataValues);
        }

        public List<TValue> Filter(Func<TValue, bool> predicate)
        {
            return Datas.Where(predicate).ToList();
        }

        public void Sort(Comparison<TValue> comparison)
        {
            Datas.Sort(comparison);
        }

        public TValue GetByIndex(int index)
        {
            return Datas.Count > index ? Datas[index] : default;
        }
    }
}