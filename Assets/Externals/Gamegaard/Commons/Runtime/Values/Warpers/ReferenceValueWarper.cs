using System;
using UnityEngine;

namespace Gamegaard.CustomValues
{
    [Serializable]
    public class ReferenceValueWarper<T>
    {
        [SerializeReference] private T value;

        public T Value
        {
            get => value;
            set => this.value = value;
        }

        public ReferenceValueWarper()
        {
        }

        public ReferenceValueWarper(T value)
        {
            this.value = value;
        }

        public static implicit operator T(ReferenceValueWarper<T> warper)
        {
            return warper.Value;
        }

        public static explicit operator ReferenceValueWarper<T>(T value)
        {
            return new ReferenceValueWarper<T> { Value = value };
        }
    }
}