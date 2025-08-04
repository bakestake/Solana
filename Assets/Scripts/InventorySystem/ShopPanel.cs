using UnityEngine;

public class ShopPanel : ItemPanel
{
    [SerializeField] private ShopManager shopManager;

    private int currentSelectedItem;

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

        Item item = LocalGameManager.Instance.currentShopContainer.slots[currentSelectedItem].item;
        if (item != null)
        {
            shopManager.SetItem(item, "Shop");
        }
        else
        {
            shopManager.SetItem(null, "Shop");
            shopManager.CurrentSelectedItem = null;
        }
    }

    public void HighlightLast()
    {
        Highlight(currentSelectedItem);
    }
}