using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class BuyABikeQuestStep : QuestStep
{
    public ItemContainer inventory;
    public Item bike;

    private void OnEnable()
    {
        GameEventsManager.Instance.inventoryEvents.onItemAdded += ItemAdded;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.inventoryEvents.onItemAdded -= ItemAdded;
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
            if (slot.item == bike)
            {
                FinishQuestStep();
                return true;
            }
        }
        return false;
    }

    private void ItemAdded(Item item, int amount)
    {
        if (item == bike)
        {
            CheckItem();
        }
    }

    private void UpdateState()
    {
        string state = "";
        string status = "Purchase a bike from the Bike Shop.";
        ChangeState(state, status);
    }


    protected override void SetQuestStepState(string state)
    {
        throw new System.NotImplementedException();
    }
}
