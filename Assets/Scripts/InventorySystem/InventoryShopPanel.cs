using UnityEngine;

public class InventoryShopPanel : ItemPanel
{
    [SerializeField] ShopManager shopManager;
    int currentSelectedItem;

    public void Reset()
    {
        foreach (InventoryButton button in buttons)
        {
            button.Highlight(false);
        }
    }

    public override void OnClick(int id)
    {
        base.OnClick(id);
        Highlight(id);
    }

    public void Highlight(int id)
    {
        buttons[currentSelectedItem].Highlight(false);
        currentSelectedItem = id;
        buttons[currentSelectedItem].Highlight(true);

        Item item = LocalGameManager.Instance.inventoryContainer.slots[currentSelectedItem].item;
        if (item != null)
        {
            shopManager.SetItem(item, "Inventory");
        }
        else
        {
            shopManager.SetItem(null, "Inventory");
            shopManager.CurrentSelectedItem = null;
        }
    }

    public void HighlightLast()
    {
        Highlight(currentSelectedItem);
    }
}