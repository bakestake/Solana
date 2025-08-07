using System.Collections;
using System.Collections.Generic;
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
        GameEventsManager.instance.miscEvents.onInteracted += Interacted;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.miscEvents.onInteracted -= Interacted;
    }

    private void Interacted(Interact go)
    {
        // Check for interaction with CaveLevel2 door or scene loader
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
