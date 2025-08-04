using System;

namespace Gamegaard.HierarchyMaster.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class HierarchyIconAttribute : Attribute
    {
        public bool IsEnabled { get; }

        public HierarchyIconAttribute(bool isEnabled = false)
        {
            IsEnabled = isEnabled;
        }
    }
}
