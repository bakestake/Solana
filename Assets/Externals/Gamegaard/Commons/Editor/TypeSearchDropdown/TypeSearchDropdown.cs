using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Gamegaard.Commons.Editor
{
    public class TypeSearchDropdown : AdvancedDropdown
    {
        private readonly Action<Type> _onTypeSelected;
        private readonly List<Type> _validTypes;

        public TypeSearchDropdown(AdvancedDropdownState state, Type baseType, Action<Type> onTypeSelected, List<Type> ignoredTypes, bool includeAbstract, bool includeInterfaces, bool includeBasicTypes, bool includeUnityObjects) : base(state)
        {
            _validTypes = GetValidTypes(baseType, ignoredTypes, includeAbstract, includeInterfaces);
            if (includeBasicTypes)
            {
                _validTypes.AddRange(new[] { typeof(int), typeof(float), typeof(bool), typeof(string) });
            }
            if (includeUnityObjects)
            {
                List<Type> unityObjectTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly =>
                {
                    try
                    {
                        return assembly.GetTypes();
                    }
                    catch
                    {
                        return Array.Empty<Type>();
                    }
                }).Where(type => typeof(UnityEngine.Object).IsAssignableFrom(type) && !type.IsAbstract).ToList();
                _validTypes.AddRange(unityObjectTypes);
            }
            _validTypes = _validTypes.Distinct().ToList();
            _onTypeSelected = onTypeSelected;
            minimumSize = new Vector2(300, 400);
        }

        public TypeSearchDropdown(AdvancedDropdownState state, List<Type> validTypes, Action<Type> onTypeSelected, bool includeBasicTypes, bool includeUnityObjects) : base(state)
        {
            _validTypes = validTypes;
            if (includeBasicTypes)
            {
                _validTypes.AddRange(new[] { typeof(int), typeof(float), typeof(bool), typeof(string) });
            }
            if (includeUnityObjects)
            {
                List<Type> unityObjectTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly =>
                {
                    try
                    {
                        return assembly.GetTypes();
                    }
                    catch
                    {
                        return Array.Empty<Type>();
                    }
                }).Where(type => typeof(UnityEngine.Object).IsAssignableFrom(type) && !type.IsAbstract).ToList();
                _validTypes.AddRange(unityObjectTypes);
            }
            _validTypes = _validTypes.Distinct().ToList();
            _onTypeSelected = onTypeSelected;
            minimumSize = new Vector2(300, 400);
        }

        public List<Type> GetValidTypes(Type baseType, List<Type> ignoredTypes, bool includeAbstract, bool includeInterfaces)
        {
            List<Type> validTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly =>
            {
                try
                {
                    return assembly.GetTypes();
                }
                catch
                {
                    return Array.Empty<Type>();
                }
            }).Where(type => baseType.IsAssignableFrom(type) && (includeAbstract || !type.IsAbstract) && (includeInterfaces || !type.IsInterface)).ToList();
            return ignoredTypes != null ? validTypes.Except(ignoredTypes).ToList() : validTypes;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            AdvancedDropdownItem root = new AdvancedDropdownItem("Select Type");
            Type[] basicTypes = { typeof(int), typeof(float), typeof(bool), typeof(string) };
            List<Type> basicTypeItems = _validTypes.Where(t => basicTypes.Contains(t)).ToList();

            if (basicTypeItems.Any())
            {
                AdvancedDropdownItem basicTypesGroup = new AdvancedDropdownItem("Basic Types");
                foreach (Type basicType in basicTypeItems)
                {
                    basicTypesGroup.AddChild(new TypeDropdownItem(basicType.Name, basicType));
                }
                root.AddChild(basicTypesGroup);
                _validTypes.RemoveAll(t => basicTypes.Contains(t));
            }

            List<Type> unityObjectTypes = _validTypes.Where(t => typeof(UnityEngine.Object).IsAssignableFrom(t)).ToList();
            if (unityObjectTypes.Any())
            {
                AdvancedDropdownItem unityObjectsGroup = new AdvancedDropdownItem("Unity Objects");
                foreach (Type unityObjectType in unityObjectTypes.OrderBy(t => t.Name))
                {
                    unityObjectsGroup.AddChild(new TypeDropdownItem(unityObjectType.Name, unityObjectType));
                }
                root.AddChild(unityObjectsGroup);
                _validTypes.RemoveAll(t => unityObjectTypes.Contains(t));
            }

            IOrderedEnumerable<IGrouping<string, Type>> groupedByNamespace = _validTypes.GroupBy(t => t.Namespace ?? "Global").OrderBy(g => g.Key);
            foreach (IGrouping<string, Type> namespaceGroup in groupedByNamespace)
            {
                string[] namespaceParts = namespaceGroup.Key.Split('.');
                AdvancedDropdownItem currentParent = root;

                foreach (string part in namespaceParts)
                {
                    AdvancedDropdownItem existingChild = currentParent.children.FirstOrDefault(c => c.name == part);
                    if (existingChild == null)
                    {
                        AdvancedDropdownItem newChild = new AdvancedDropdownItem(part);
                        currentParent.AddChild(newChild);
                        currentParent = newChild;
                    }
                    else
                    {
                        currentParent = existingChild;
                    }
                }

                foreach (Type type in namespaceGroup.OrderBy(t => t.Name))
                {
                    currentParent.AddChild(new TypeDropdownItem(type.Name, type));
                }
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            if (item is TypeDropdownItem typeItem)
            {
                _onTypeSelected?.Invoke(typeItem.Type);
            }
        }
    }
}
