using UnityEngine;

public class AltarOfTheDeadQuestStep2 : QuestStep
{
    private bool hasEnteredLevel2 = false;

    private void Start()
    {
        UpdateState();
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.miscEvents.OnInteracted += Interacted;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.miscEvents.OnInteracted -= Interacted;
    }

    private void Interacted(Interact go)
    {
        if (!hasEnteredLevel2)
        {
            if (go.name.Contains("CaveLevel2") ||
                go.gameObject.name == "CaveLevel2Door")
            {
                hasEnteredLevel2 = true;
                FinishQuestStep();
            }
        }
    }

    private void UpdateState()
    {
        string state = hasEnteredLevel2 ? "completed" : "ongoing";
        string status = "Go deeper into the cave.";
        ChangeState(state, status);
    }

    protected override void SetQuestStepState(string state)
    {
        hasEnteredLevel2 = state == "completed";
    }
}
