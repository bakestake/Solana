using Febucci.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private InventoryShopPanel inventoryShopPanel;
    [SerializeField] private ShopPanel shopPanel;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [SerializeField] private TextMeshProUGUI itemBuyPriceText;
    [SerializeField] private TextMeshProUGUI itemSellPriceText;
    [SerializeField] private TypewriterByCharacter itemNameTypewriter;
    [SerializeField] private TypewriterByCharacter itemDescriptionTypewriter;
    [SerializeField] private CanvasGroup itemInfoBox;
    [SerializeField] private Button buyItemButton;
    [SerializeField] private Button sellItemButton;

    private Item currentSelectedItem;
    private string currentPanel;

    public InventoryShopPanel InventoryShopPanel => inventoryShopPanel;
    public Item CurrentSelectedItem
    {
        get => currentSelectedItem;
        set => currentSelectedItem = value;
    }

    private void Start()
    {
        ResetUI();
        InitButtons();
    }

    private void InitButtons()
    {
        buyItemButton.onClick.AddListener(() => BuyItemButton());
        sellItemButton.onClick.AddListener(() => SellItemButton());
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
        itemNameText.text = item.ItemName;
        itemDescriptionText.text = item.Description;
        itemNameTypewriter.ShowText(item.ItemName);
        itemDescriptionTypewriter.ShowText(item.Description);
        itemBuyPriceText.text = item.BuyPrice.ToString();
        itemSellPriceText.text = item.SellPrice.ToString();

        itemInfoBox.alpha = 1;
        HandleButtons();
    }

    public void ResetUI()
    {
        itemNameText.text = string.Empty;
        itemDescriptionText.text = string.Empty;
        buyItemButton.interactable = false;
        sellItemButton.interactable = false;
        itemInfoBox.alpha = 0;
        currentPanel = null;
        currentSelectedItem = null;
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

    // change so that API change to gold is validated before item is bought
    public void BuyItem(Item item)
    {
        if (item != null && PlayerCurrency.instance.CanAfford(item.BuyPrice) && item.Marketable)
        {
            LocalGameManager.Instance.inventoryContainer.Add(item, 1);
            PlayerCurrency.instance.SubtractGold(item.BuyPrice);
            inventoryShopPanel.Show();

            SoundManager.Instance.PlayRandomFromList(SoundManager.Instance.coinSounds);
        }
    }

    // change so that API change to gold is validated before item is sold
    public void SellItem(Item item)
    {
        if (item != null && item.Marketable)
        {
            LocalGameManager.Instance.inventoryContainer.Remove(item, 1);
            PlayerCurrency.instance.AddGold(item.SellPrice);
            inventoryShopPanel.Show();

            SoundManager.Instance.PlayRandomFromList(SoundManager.Instance.coinSounds);
        }
    }

    private void HandleButtons()
    {
        if (currentPanel == "Shop")
        {
            buyItemButton.interactable = true;
            sellItemButton.interactable = false;

            if (currentSelectedItem != null)
            {
                if (!PlayerCurrency.instance.CanAfford(currentSelectedItem.BuyPrice) || currentSelectedItem.Marketable == false)
                {
                    buyItemButton.interactable = false;
                }
            }
        }
        else if (currentPanel == "Inventory")
        {
            buyItemButton.interactable = false;
            sellItemButton.interactable = true;

            if (currentSelectedItem != null && !currentSelectedItem.Marketable)
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
}
