using Gamegaard.CustomValues;
using System;

namespace Gamegaard.SerializableAttributes
{
    [Serializable]
    public class AttributesDictionary : SerializableDictionary<string, IAttributeValue>
    {

    }
}