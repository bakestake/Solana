using System;

public class InventoryEvents
{
    public event Action<Item, int> onItemAdded;
    public void ItemAdded(Item item, int amount)
    {
        if (onItemAdded != null)
        {
            onItemAdded(item, amount);
        }
    }

    public event Action<Item, int> onItemRemoved;
    public void ItemRemoved(Item item, int amount)
    {
        if (onItemRemoved != null)
        {
            onItemRemoved(item, amount);
        }
    }
}
