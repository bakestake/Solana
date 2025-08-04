using Gamegaard.FarmSystem;
using UnityEngine;

[System.Serializable]
public class BakelandPlantReward : PlantReward
{
    [SerializeField] private Item item;
    [Min(1)]
    [SerializeField] private int amount;

    public override string GetRewardDescription(Plant plant, object receiver)
    {
        return $"{item.ItemName} {amount}x";
    }

    public override void ReceiveReward(Plant plant, object receiver)
    {
        Debug.Log("Added");
        if (LocalGameManager.Instance.inventoryContainer != null)
        {
            LocalGameManager.Instance.inventoryContainer.Add(item, amount);
            PickUpItemFunctionName.mintFunctionName = item.MintFunction;
        }
        else
        {
            Debug.LogWarning("No inventory container attached to the GameManager!");
        }
    }
}