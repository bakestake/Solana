using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryController : MonoBehaviour
{
    [SerializeField] Animator inventoryAnimator;
    [SerializeField] InventoryPanel panel;
    [SerializeField] GameObject toolbarPanel;
    [SerializeField] GameObject shopPanel;
    // [SerializeField] ShopController shopController;
    [SerializeField] KeyCode key;

    public bool isActive;

    // Update is called once per frame
    void Update()
    {
        if (LocalGameManager.Instance.canUseKeybinds && Input.GetKeyDown(key))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        // if (shopController.isActive) { return; }
        if (isActive)
        {
            isActive = false;
            inventoryAnimator.SetTrigger("out");
            // panel.SetActive(false);
            toolbarPanel.SetActive(true);
            PlayerController.canMove = true;
            Debug.Log("Can Move true");
            // GameManager.instance.toolsCharacterController.canUse = true;

            LocalGameManager.Instance.dragAndDropController.PutItemBack();

            SoundManager.instance.PlaySfx(SoundManager.instance.inventoryOut);
        }
        else
        {
            isActive = true;
            inventoryAnimator.SetTrigger("in");
            panel.Show();
            toolbarPanel.SetActive(false);
            // shopPanel.SetActive(false);
            // GameManager.instance.toolsCharacterController.canUse = false;
            PlayerController.canMove = false;
            SoundManager.instance.PlaySfx(SoundManager.instance.inventoryIn);
        }

    }

    private void OnApplicationQuit()
    {
        // GameManager.instance.dragAndDropController.PutItemBack();
    }
}
