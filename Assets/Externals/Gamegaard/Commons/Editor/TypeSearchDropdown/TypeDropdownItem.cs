using System;
using UnityEditor.IMGUI.Controls;

namespace Gamegaard.Commons.Editor
{
    public class TypeDropdownItem : AdvancedDropdownItem
    {
        public Type Type { get; private set; }

        public TypeDropdownItem(string name, Type type) : base(name)
        {
            Type = type;
        }
    }
}