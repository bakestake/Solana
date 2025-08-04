using UnityEngine;
using System;

namespace Gamegaard
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class TypeSearchAttribute : PropertyAttribute
    {
        public Type BaseType { get; }

        public TypeSearchAttribute(Type baseType)
        {
            BaseType = baseType;
        }
    }
}
