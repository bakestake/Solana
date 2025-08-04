using Gamegaard.Singleton;
using UnityEngine;

public class GlobalShopController : MonoBehaviourSingleton<GlobalShopController>
{
    [Header("General References")]
    [SerializeField] private GameObject toolbarPanel;
    [SerializeField] private InventoryController inventoryController;

    [Header("Bike Shop References")]
    [SerializeField] private ShopManager bikeShopManager;
    [SerializeField] private GameObject bikeShopPanel;
    [SerializeField] private ItemContainer bikeShopContainer;
    [SerializeField] private Animator bikeShopAnimator;

    private int selectedItem;
    private Shop currentActiveShop;

    public Item GetItem => LocalGameManager.Instance.currentShopContainer.slots[selectedItem].item;

    public void SetShop(Shop shop)
    {
        switch (shop)
        {
            case Shop.Bike:
                LocalGameManager.Instance.currentShopContainer = bikeShopContainer;
                bikeShopAnimator.SetTrigger("in");
                bikeShopManager.InventoryShopPanel.Show();
                PlayerController.canMove = false;
                toolbarPanel.SetActive(false);
                LocalGameManager.Instance.dragAndDropController.PutItemBack();
                break;
            case Shop.Candyman:
                break;
            default:
                break;
        }
    }

    public void CloseShop(Shop shop)
    {
        switch (shop)
        {
            case Shop.Bike:
                LocalGameManager.Instance.currentShopContainer = null;
                bikeShopAnimator.SetTrigger("out");
                toolbarPanel.SetActive(true);
                bikeShopManager.ResetUI();
                PlayerController.canMove = true;
                LocalGameManager.Instance.dragAndDropController.PutItemBack();
                break;
            case Shop.Candyman:
                break;
            default:
                break;
        }

    }
}

public enum Shop
{
    Bike,
    Candyman
}