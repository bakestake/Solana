using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollectHoneycombsQuestStep : QuestStep
{
    private int honeycombsCollected = 0;
    public int honeycombsToComplete = 2;
    public Item honeycomb;

    private void Start()
    {
        UpdateState();
    }

    private void OnEnable()
    {
        GameEventsManager.instance.inventoryEvents.onItemAdded += ItemAdded;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.inventoryEvents.onItemAdded -= ItemAdded;
    }

    private void ItemAdded(Item item, int amount)
    {
        Debug.Log(item.Name);
        if (item == honeycomb)
        {
            if (honeycombsCollected < honeycombsToComplete)
            {
                honeycombsCollected += amount;
                // coinsCollected++;
                UpdateState();
            }

            if (honeycombsCollected >= honeycombsToComplete)
            {
                FinishQuestStep();
            }
        }
    }

    private void UpdateState()
    {
        string state = honeycombsCollected.ToString();
        string status = "Collected " + honeycombsCollected + " / " + honeycombsToComplete + " honeycombs.";
        ChangeState(state, status);
    }

    protected override void SetQuestStepState(string state)
    {
        this.honeycombsCollected = System.Int32.Parse(state);
        UpdateState();
    }
}
