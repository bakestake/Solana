using System.Collections.Generic;
using UnityEngine;

namespace Gamegaard.CustomValues.Database
{
    public abstract class ReferenceScriptableDatabase<TValue> : ListScriptableDatabase<TValue>
    {
        [SerializeReference] private List<TValue> datas = new List<TValue>();

        public sealed override List<TValue> Datas => datas;
    }
}