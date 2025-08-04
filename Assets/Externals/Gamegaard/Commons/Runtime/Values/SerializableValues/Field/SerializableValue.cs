using System;
using UnityEngine;

namespace Gamegaard.CustomValues
{
    [Serializable]
    public class SerializableValue<T> : ISerializationCallbackReceiver
    {
        [SerializeField] private UnityEngine.Object unityReference;
        [SerializeReference] private T serializedReference;
        [SerializeField] private string currentType;

        private T cachedValue;

        public string CurrentType => currentType;
        public T Value
        {
            get => cachedValue;
            set => SetValue(value);
        }

        public SerializableValue(T value)
        {
            SetValue(value);
        }

        public void OnBeforeSerialize()
        {
            UpdateSerializedReferences(cachedValue);
        }

        public void OnAfterDeserialize()
        {
            if (unityReference is T unityObj)
            {
                cachedValue = unityObj;
                return;
            }

            cachedValue = serializedReference;
        }

        private void SetValue(T value)
        {
            UpdateSerializedReferences(value);
            cachedValue = value;
        }

        private void UpdateSerializedReferences(T value)
        {
            if (value is UnityEngine.Object obj)
            {
                unityReference = obj;
                serializedReference = default;
            }
            else
            {
                unityReference = null;
                serializedReference = value;
            }
        }

        public void SetCurrentType(Type type)
        {
            if (type != null)
            {
                currentType = type.AssemblyQualifiedName;
            }
        }

        public static implicit operator T(SerializableValue<T> serializableValue)
        {
            return serializableValue.cachedValue;
        }

        public static implicit operator SerializableValue<T>(T value)
        {
            return new SerializableValue<T>(value);
        }
    }
}