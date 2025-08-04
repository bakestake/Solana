using Gamegaard.CustomValues;
using Gamegaard.CustomValues.Database;
using UnityEngine;

namespace Gamegaard.SerializableAttributes
{
    [CreateAssetMenu(fileName = "AttributesDatabase", menuName = "InventorySystem/AttributesDatabase")]
    public class AttributesDatabase : ScriptableDictionaryDatabase<IAttributeValue>
    {
        [SerializeReference] private AttributesDictionary datas = new AttributesDictionary();
        protected override SerializableDictionary<string, IAttributeValue> DictionaryDatas => datas;
    }
}