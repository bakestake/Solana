using UnityEngine;

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
        if (Input.GetKeyDown(key) && PlayerController.canInteract)
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
            // GameManager.instance.toolsCharacterController.canUse = true;

            LocalGameManager.NotNullInstance.dragAndDropController.PutItemBack();

            SoundManager.Instance.PlaySfx(SoundManager.Instance.inventoryOut);
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
            SoundManager.Instance.PlaySfx(SoundManager.Instance.inventoryIn);
        }

    }

    private void OnApplicationQuit()
    {
        // GameManager.instance.dragAndDropController.PutItemBack();
    }
}
