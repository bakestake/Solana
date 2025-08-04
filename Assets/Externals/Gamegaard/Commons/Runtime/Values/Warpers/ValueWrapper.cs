using System;
using UnityEngine;

namespace Gamegaard.CustomValues
{
    [Serializable]
    public class ValueWrapper<T>
    {
        [SerializeField] private T value;

        public T Value
        {
            get => value;
            set => this.value = value;
        }

        public ValueWrapper()
        {
        }

        public ValueWrapper(T value)
        {
            this.value = value;
        }

        public static implicit operator T(ValueWrapper<T> warper)
        {
            return warper.Value;
        }

        public static implicit operator ValueWrapper<T>(T value)
        {
            return new ValueWrapper<T> { Value = value };
        }
    }

    [Serializable]
    public class RawValueWrapper
    {
        [SerializeField] private string storedType;
        [SerializeReference] private object value;

        public object Value => value;

        public RawValueWrapper()
        {
        }

        public RawValueWrapper(object value)
        {
            SetValue(value);
        }

        public void SetValue(object newValue)
        {
            if (newValue != null)
            {
                storedType = newValue.GetType().AssemblyQualifiedName;
                value = newValue;
            }
        }

        public Type GetStoredType()
        {
            return string.IsNullOrEmpty(storedType) ? null : Type.GetType(storedType);
        }
    }
}