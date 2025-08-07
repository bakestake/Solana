using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryShopPanel : ItemPanel
{
    [SerializeField] ShopManager shopManager;

    private void Start()
    {
        Init();
    }

    public override void OnClick(int id)
    {
        Highlight(id);
        SoundManager.instance.PlayRandomFromList(SoundManager.instance.clickSounds);
    }

    int currentSelectedItem;

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
            shopManager.currentSelectedItem = null;
        }
    }

    public void HighlightLast()
    {
        Highlight(currentSelectedItem);
    }

    public void Reset()
    {
        foreach (InventoryButton button in buttons)
        {
            button.Highlight(false);
        }
    }
}
