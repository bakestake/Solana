using System.Collections.Generic;
using UnityEngine;

namespace Bakeland
{
    public class ItemPickupPopup : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private GameObject elementPrefab;

        private List<ItemPickupPopupElement> elements = new();

        private void Awake()
        {
            // clear all previous references
            for (int i = 0; i < content.childCount; i++)
            {
                Destroy(content.GetChild(i).gameObject);
            }
        }

        private void OnEnable()
        {
            GameEventsManager.Instance.inventoryEvents.onItemAdded += OnItemAdded;
        }

        private void OnDisable()
        {
            if (!GameEventsManager.HasInstance) return;
            GameEventsManager.Instance.inventoryEvents.onItemAdded -= OnItemAdded;
        }

        private void OnItemAdded(Item item, int count)
        {
            ItemPickupPopupElement newElement = Instantiate(elementPrefab, content).GetComponent<ItemPickupPopupElement>();
            newElement.Initialize(item.Icon, item.ItemName);
            elements.Add(newElement);
        }
    }
}