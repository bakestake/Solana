using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard.CustomValues.Database
{
    public abstract class ScriptableDatabase<TValue> : ListScriptableDatabase<TValue>
    {
        [SerializeField] private List<TValue> datas;
        public sealed override List<TValue> Datas => datas;
    }
}