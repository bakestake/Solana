using System.Collections;
using System.Collections.Generic;
using Febucci.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public Item currentSelectedItem;
    public string currentPanel;
    [Header("Panels")]
    public InventoryShopPanel inventoryShopPanel;
    public ShopPanel shopPanel;
    [Header("References")]
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public TextMeshProUGUI itemBuyPriceText;
    public TextMeshProUGUI itemSellPriceText;
    public TypewriterByCharacter itemNameTypewriter;
    public TypewriterByCharacter itemDescriptionTypewriter;
    public CanvasGroup itemInfoBox;
    public Button buyItemButton;
    public Button sellItemButton;

    private void Start()
    {
        ResetUI();
    }

    public void SetItem(Item item, string panel)
    {
        if (item != null)
        {
            currentSelectedItem = item;
            currentPanel = panel;
            PopulateUI(currentSelectedItem);
        }
        else
        {
            ResetUI();
        }
    }

    private void PopulateUI(Item item)
    {
        // itemNameText.text = item.Name;
        // itemDescriptionText.text = item.description;
        itemNameTypewriter.ShowText(item.Name);
        itemDescriptionTypewriter.ShowText(item.description);
        itemBuyPriceText.text = item.buyPrice.ToString();
        itemSellPriceText.text = item.sellPrice.ToString();

        itemInfoBox.alpha = 1;
        HandleButtons();
    }

    public void BuyItemButton()
    {
        if (currentSelectedItem != null)
        {
            BuyItem(currentSelectedItem);
            shopPanel.HighlightLast();
        }
    }

    public void SellItemButton()
    {
        if (currentSelectedItem != null)
        {
            SellItem(currentSelectedItem);
            inventoryShopPanel.HighlightLast();
        }
    }

    public void BuyItem(Item item)
    {
        if (item != null)
        {
            if (PlayerCurrency.instance.CanAfford(item.buyPrice) && item.marketable)
            {
                LocalGameManager.Instance.inventoryContainer.Add(item, 1);
                // LocalGameManager.Instance.currentShopContainer.Remove(item, 1);
                PlayerCurrency.instance.SubtractGold(item.buyPrice);
                SoundManager.instance.PlayRandomFromList(SoundManager.instance.coinSounds);
            }
        }
    }

    public void SellItem(Item item)
    {
        if (item != null)
        {
            if (item.marketable)
            {
                LocalGameManager.Instance.inventoryContainer.Remove(item, 1);
                PlayerCurrency.instance.AddGold(item.sellPrice);

                SoundManager.instance.PlayRandomFromList(SoundManager.instance.coinSounds);
            }
        }
    }

    private void HandleButtons()
    {
        if (currentPanel == "Shop")
        {
            buyItemButton.interactable = true;
            sellItemButton.interactable = false;

            if (currentSelectedItem != null && !PlayerCurrency.instance.CanAfford(currentSelectedItem.buyPrice))
            {
                buyItemButton.interactable = false;
            }
        }
        else if (currentPanel == "Inventory")
        {
            buyItemButton.interactable = false;
            sellItemButton.interactable = true;

            if (currentSelectedItem != null && !currentSelectedItem.marketable)
            {
                sellItemButton.interactable = false;
            }
        }
        else
        {
            buyItemButton.interactable = false;
            sellItemButton.interactable = false;
        }
    }

    public void ResetUI()
    {
        buyItemButton.interactable = false;
        sellItemButton.interactable = false;
        itemInfoBox.alpha = 0;
        currentPanel = null;
        currentSelectedItem = null;
    }
}
