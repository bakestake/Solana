using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalShopController : MonoBehaviour
{
    public static GlobalShopController instance;
    public Shop currentActiveShop;

    [Header("General References")]
    [SerializeField] GameObject toolbarPanel;
    [SerializeField] InventoryController inventoryController;

    [Header("Bike Shop References")]
    [SerializeField] ShopManager bikeShopManager;
    [SerializeField] GameObject bikeShopPanel;
    [SerializeField] ItemContainer bikeShopContainer;
    [SerializeField] Animator bikeShopAnimator;

    int selectedItem;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Item GetItem
    {
        get
        {
            return LocalGameManager.Instance.currentShopContainer.slots[selectedItem].item;
        }
    }

    public void SetShop(Shop shop)
    {
        switch (shop)
        {
            case Shop.Bike:
                LocalGameManager.Instance.currentShopContainer = bikeShopContainer;
                bikeShopAnimator.SetTrigger("in");
                bikeShopManager.inventoryShopPanel.Show();
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
                Debug.Log("Can Move true");
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