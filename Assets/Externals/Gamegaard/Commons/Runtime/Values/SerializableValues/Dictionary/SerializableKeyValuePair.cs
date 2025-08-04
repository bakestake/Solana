namespace Gamegaard.CustomValues
{
    [System.Serializable]
    public struct SerializableKeyValuePair<KeyType, ValueType>
    {
        public KeyType key;
        public SerializableValue<ValueType> value;

        public SerializableKeyValuePair(KeyType key, ValueType value)
        {
            this.key = key;
            this.value = new SerializableValue<ValueType>(value);
        }
    }
}
