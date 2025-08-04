using UnityEngine;

public class ExploreTheCaveQuestStep3 : QuestStep
{
    [SerializeField] private Item chainsawItem;

    private void Start()
    {
        UpdateState();

        if (LocalGameManager.Instance.CheckForItem(chainsawItem)) FinishQuestStep();
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.inventoryEvents.onItemAdded += ItemAdded;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.inventoryEvents.onItemAdded -= ItemAdded;
    }

    private void ItemAdded(Item item, int id)
    {
        if (item == chainsawItem) FinishQuestStep();
    }

    private void UpdateState()
    {
        string state = "";
        string status = "Grab the chainsaw left in the cave.";
        ChangeState(state, status);
    }

    protected override void SetQuestStepState(string state)
    {
        // throw new System.NotImplementedException();
    }
}
