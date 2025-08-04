using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using System;

namespace Gamegaard.SerializableAttributes.Editor
{
    public class AttributeSearchDropdown : AdvancedDropdown
    {
        private event Action<string, BasicSerializableValuesTypes> onAttributeSelected;
        private event Action onCreateNewAttribute;
        private readonly List<KeyValuePair<string, List<KeyValuePair<string, BasicSerializableValuesTypes>>>> availableAttributes;
        private const string newValueName = "New Attribute";

        public AttributeSearchDropdown(AdvancedDropdownState state, List<KeyValuePair<string, List<KeyValuePair<string, BasicSerializableValuesTypes>>>> attributes, Action<string, BasicSerializableValuesTypes> onAttributeSelectedCallback, Action onCreateNewAttributeCallback) : base(state)
        {
            availableAttributes = attributes;
            onAttributeSelected = onAttributeSelectedCallback;
            onCreateNewAttribute = onCreateNewAttributeCallback;
            minimumSize = new Vector2(300, 400);
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            AdvancedDropdownItem root = new AdvancedDropdownItem("Select Attribute");

            AdvancedDropdownItem newAttributeItem = new AdvancedDropdownItem(newValueName);
            root.AddChild(newAttributeItem);

            foreach (KeyValuePair<string, List<KeyValuePair<string, BasicSerializableValuesTypes>>> group in availableAttributes)
            {
                AdvancedDropdownItem groupItem = new AdvancedDropdownItem(group.Key);

                foreach (KeyValuePair<string, BasicSerializableValuesTypes> attribute in group.Value)
                {
                    string displayName = $"{attribute.Key} ({attribute.Value})";
                    AdvancedDropdownItem attributeItem = new AdvancedDropdownItem(displayName);
                    groupItem.AddChild(attributeItem);
                }

                root.AddChild(groupItem);
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            if (item.name == newValueName)
            {
                onCreateNewAttribute?.Invoke();
            }
            else
            {
                foreach (KeyValuePair<string, List<KeyValuePair<string, BasicSerializableValuesTypes>>> group in availableAttributes)
                {
                    foreach (KeyValuePair<string, BasicSerializableValuesTypes> attribute in group.Value)
                    {
                        string displayName = $"{attribute.Key} ({attribute.Value})";
                        if (item.name == displayName)
                        {
                            onAttributeSelected?.Invoke(attribute.Key, attribute.Value);
                            return;
                        }
                    }
                }
            }
        }
    }
}
