using UnityEngine;

[System.Serializable]
public class ItemInteractionCondition : IInteractionCondition
{
    [SerializeField] private Item itemToCheck;
    [SerializeField] private int requiredAmount;
    [SerializeField] private bool destroyOnIntraction;
    [SerializeField] private bool mustBeSelected;

    public ItemInteractionCondition()
    { }

    public bool CanInteract()
    {
        int totalItem = 0;

        if (mustBeSelected)
        {
            ToolbarController toolbarController = LocalGameManager.Instance.PlayerController.GetComponent<ToolbarController>();
            Item item = toolbarController.SelectedItem;
            return item == itemToCheck;
        }
        else
        {
            foreach (ItemSlot slot in LocalGameManager.Instance.inventoryContainer.slots)
            {
                if (slot.item == itemToCheck)
                {
                    totalItem += slot.count;

                    if (slot.item.Unique)
                    {
                        totalItem = 99;
                    }
                }
            }
            return totalItem >= requiredAmount;
        }
    }

    public void OnInteracted()
    {
        if (destroyOnIntraction)
        {
            LocalGameManager.Instance.inventoryContainer.Remove(itemToCheck, requiredAmount);
        }
    }
}