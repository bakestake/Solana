using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemSlot
{
    public Item item;
    public int count;

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

    public void Add(Item item, int count = 1)
    {
        isDirty = true;
        if (item.unique)
        {
            ItemSlot itemSlot = slots.Find(x => x.item == item);
            if (itemSlot != null)
            {
                return;
            }
        }

        if (item.stackable)
        {
            ItemSlot itemSlot = slots.Find(x => x.item == item);
            if (itemSlot != null)
            {
                if (itemSlot.count == item.maxStack)
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
            }
        }

        GameEventsManager.instance.inventoryEvents.ItemAdded(item, count);
    }

    public void Remove(Item itemToRemove, int count = 1)
    {
        isDirty = true;

        if (itemToRemove.stackable)
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
    }

    public bool ContainsItem(Item itemToCheck)
    {
        ItemSlot itemSlot = slots.Find(x => x.item == itemToCheck);
        if (itemSlot == null)
        {
            return false;
        }
        else return true;

    }

    public void ClearItems()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].Clear();
        }
    }
}
