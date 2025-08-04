using UnityEngine;

[CreateAssetMenu(menuName = "Data/Item Action/Use Food")]
public class UseFood : ItemAction
{
    [SerializeField] private int healAmount;

    public override void OnItemUsed(Item usedItem, ItemContainer inventory)
    {
        if (PlayerController.Instance.TryGetComponent(out Health playerHealth))
        {
            playerHealth.Heal(healAmount);
            LocalGameManager.Instance.inventoryContainer.Remove(usedItem);
        }
    }
}