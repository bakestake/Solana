using System;
using System.Collections.Generic;
using System.Linq;
using Bakeland;
using UnityEngine;

[Serializable]
public class ItemSlot
{
    [HideInInspector] public string editor_itemName = "Test";

    public Item item;
    public int count;

    public bool IsEmpty => item == null || count == 0;

    public void Copy(ItemSlot slot)
    {
        item = slot.item;
        count = slot.count;
    }

    public void Set(Item item, int count)
    {
        this.item = item;
        this.count = count;
    }

    public void Clear()
    {
        item = null;
        count = 0;
    }
}

[CreateAssetMenu(menuName = "Data/Item Container")]
public class ItemContainer : ScriptableObject
{
    public List<ItemSlot> slots;
    public bool isDirty;

    private void OnValidate()
    {
        foreach (var slot in slots)
        {
            slot.editor_itemName = slot.IsEmpty ? string.Empty : slot.item.ItemName;
        }
    }

    public void Add(Item item, int count = 1)
    {
        Debug.Log($"{name}:adding item {count} {item.ItemName}");
        if (!slots.Any(x => x.item == null))
        {
            Debug.LogWarning($"ItemContainer: INVENTORY IS FULL when trying to loading item {item.name}");
        }

        isDirty = true;
        if (item.Unique)
        {
            if (slots.Any(x => x.item == item))
            {
                // skip duplicate unique items
                Debug.Log($"{name}:skipping duplicate unique item {item.ItemName}");
                return;
            }
        }

        if (item.Stackable)
        {
            ItemSlot itemSlot = slots.Find(x => x.item == item);
            if (itemSlot != null)
            {
                if (itemSlot.count == item.MaxStack)
                {
                    itemSlot = slots.Find(x => x.item == null);
                    if (itemSlot != null)
                    {
                        itemSlot.item = item;
                        itemSlot.count = count;
                    }
                }
                itemSlot.count += count;
            }
            else
            {
                itemSlot = slots.Find(x => x.item == null);
                if (itemSlot != null)
                {
                    itemSlot.item = item;
                    itemSlot.count = count;
                }
            }
        }
        else
        {
            ItemSlot itemSlot = slots.Find(x => x.item == null);
            if (itemSlot != null)
            {
                itemSlot.item = item;
                itemSlot.count = count;
            }
        }

        if (item.IsMintable) AddToInventoryAPI(item, count);
        GameEventsManager.Instance.inventoryEvents.ItemAdded(item, count);
    }

    private async void AddToInventoryAPI(Item item, int count)
    {
        await InventoryApi.MintItem(WalletConnectScript.connectedWalletAddress, item.UniqueID, count);
    }

    public async void Remove(Item itemToRemove, int count = 1)
    {
        isDirty = true;

        if (itemToRemove.Stackable)
        {
            ItemSlot itemSlot = slots.Find(x => x.item == itemToRemove);
            if (itemSlot == null) { return; }
            itemSlot.count -= count;
            if (itemSlot.count <= 0)
            {
                itemSlot.Clear();
            }
        }
        else
        {
            while (count > 0)
            {
                count -= 1;

                ItemSlot itemSlot = slots.Find(x => x.item == itemToRemove);
                if (itemSlot == null) { return; }
                itemSlot.Clear();
            }
        }

        if (itemToRemove.IsMintable) await InventoryApi.BurnItem(WalletConnectScript.connectedWalletAddress, itemToRemove.UniqueID, count);
    }

    public bool ContainsItem(Item targetItem)
    {
        ItemSlot itemSlot = slots.Find(x => x.item == targetItem);
        if (itemSlot == null)
        {
            return false;
        }
        else return true;
    }

    public int ContainsItemQuantity(Item targetItem)
    {
        foreach (var slot in slots)
        {
            if (slot.item == targetItem) return slot.count;
        }

        return 0;
    }

    public void ClearItems()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].Clear();
        }
    }

    public void LoadInventory(Bakeland.InventoryResponse apiInventory)
    {
        foreach (var apiItem in apiInventory.inventory)
        {
            if (!slots.Any(x => x.item == null))
            {
                Debug.LogWarning($"ItemContainer: INVENTORY IS FULL when trying to loading item {apiItem.name}");
                break;
            }

            int count = int.Parse(apiItem.balance);
            Item item = ItemDatabaseManager.Instance.Find(int.Parse(apiItem.id));

            if (item.Unique)
            {
                if (slots.Any(x => x.item == item))
                {
                    // skip duplicate unique items
                    Debug.Log($"{name}:skipping duplicate unique item {item.ItemName}");
                    return;
                }
            }

            if (item.Stackable)
            {
                ItemSlot itemSlot = slots.Find(x => x.item == item);
                if (itemSlot != null)
                {
                    if (itemSlot.count == item.MaxStack)
                    {
                        itemSlot = slots.Find(x => x.item == null);
                        if (itemSlot != null)
                        {
                            itemSlot.item = item;
                            itemSlot.count = count;
                        }
                    }
                    itemSlot.count += count;
                }
                else
                {
                    itemSlot = slots.Find(x => x.item == null);
                    if (itemSlot != null)
                    {
                        itemSlot.item = item;
                        itemSlot.count = count;
                    }
                }
            }
            else
            {
                ItemSlot itemSlot = slots.Find(x => x.item == null);
                if (itemSlot != null)
                {
                    itemSlot.item = item;
                    itemSlot.count = count;
                }
            }
        }
    }
}
