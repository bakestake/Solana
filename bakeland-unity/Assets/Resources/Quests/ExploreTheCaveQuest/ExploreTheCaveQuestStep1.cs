using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreTheCaveQuestStep1 : QuestStep
{
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
        if (go.gameObject.name == "CaveEntrance")
        {
            StartCoroutine(FinishQuestStepAfterTransition());
        }
    }

    private IEnumerator FinishQuestStepAfterTransition()
    {
        yield return new WaitForSeconds(0.5f);
        FinishQuestStep();
    }

    private void UpdateState()
    {
        string state = "";
        string status = "Find the cave entrance.";
        ChangeState(state, status);
    }

    protected override void SetQuestStepState(string state)
    {
        // throw new System.NotImplementedException();
    }
}
