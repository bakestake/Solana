using UnityEngine;

public class CollectHoneycombsQuestStep : QuestStep
{
    private int honeycombsCollected = 0;
    public int honeycombsToComplete = 2;
    public Item honeycomb;

    private void Start()
    {
        // check how many honeycombs the player has when starting the quest
        honeycombsCollected = LocalGameManager.Instance.CheckForItemQuantity(honeycomb);

        UpdateState();

        // check if the player can already turn in the quest just as it started (enough honeycombs)
        if (honeycombsCollected >= honeycombsToComplete) FinishQuestStep();
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.inventoryEvents.onItemAdded += ItemAdded;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.inventoryEvents.onItemAdded -= ItemAdded;
    }

    private void ItemAdded(Item item, int amount)
    {
        Debug.Log($"{name}:added {item.ItemName}");
        if (item == honeycomb)
        {
            if (honeycombsCollected < honeycombsToComplete)
            {
                honeycombsCollected += amount;
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
        if (int.TryParse(state, out int result))
        {
            honeycombsCollected = result;
        }
        UpdateState();
    }
}