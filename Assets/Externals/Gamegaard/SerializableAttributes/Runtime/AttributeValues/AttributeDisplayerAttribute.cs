using System;
using UnityEngine;

namespace Gamegaard.SerializableAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class AttributeDisplayerAttribute : PropertyAttribute
    {
        public string Label { get; }

        public AttributeDisplayerAttribute(string label = "Attributes")
        {
            Label = label;
        }
    }
}