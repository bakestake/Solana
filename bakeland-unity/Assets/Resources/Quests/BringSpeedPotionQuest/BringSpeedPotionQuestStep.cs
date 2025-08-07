using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class BringSpeedPotionQuestStep : QuestStep
{
    public ItemContainer inventory;
    public Item speedPotion;
    public int speedPotionCollected;
    public int totalSpeedPotionAmountToComplete;

    private void OnEnable()
    {
        GameEventsManager.instance.inventoryEvents.onItemAdded += ItemAdded;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.inventoryEvents.onItemAdded -= ItemAdded;
    }

    private void Start()
    {
        CheckItem();
        UpdateState();
    }

    public bool CheckItem()
    {
        foreach (ItemSlot slot in inventory.slots)
        {
            if (slot.item == speedPotion)
            {
                if (slot.count < totalSpeedPotionAmountToComplete)
                {
                    speedPotionCollected = slot.count;
                    UpdateState();
                }
                if (slot.count >= totalSpeedPotionAmountToComplete)
                {
                    FinishQuestStep();
                }
                return true;
            }
        }
        return false;
    }

    private void ItemAdded(Item item, int amount)
    {
        if (item == speedPotion)
        {
            CheckItem();
        }
    }

    private void UpdateState()
    {
        string state = speedPotionCollected.ToString();
        string status = "You have " + speedPotionCollected + " / " + totalSpeedPotionAmountToComplete + " speed potions.";
        ChangeState(state, status);
    }


    protected override void SetQuestStepState(string state)
    {
        this.speedPotionCollected = System.Int32.Parse(state);
        UpdateState();
    }
}
