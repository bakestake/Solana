using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreTheCaveQuestStep2 : QuestStep
{
    private void Start()
    {
        UpdateState();
    }

    private void OnEnable()
    {
        GameEventsManager.instance.miscEvents.onDialogueEnded += Interacted;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.miscEvents.onDialogueEnded -= Interacted;
    }

    private void Interacted(DialogueTrigger go)
    {
        if (go.GetComponent<DialogueTrigger>() != null)
        {
            if (go.GetComponent<DialogueTrigger>().defaultDialogue.actors[0].Name.Contains("Bozito"))
            {
                FinishQuestStep();
            }
        }
    }

    private void UpdateState()
    {
        string state = "";
        string status = "Talk with Bozito.";
        ChangeState(state, status);
    }

    protected override void SetQuestStepState(string state)
    {
        // throw new System.NotImplementedException();
    }
}
