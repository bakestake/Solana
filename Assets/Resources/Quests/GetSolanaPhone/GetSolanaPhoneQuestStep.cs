using System;
using UnityEngine;

public class GetSolanaPhoneQuestStep : QuestStep
{
    private void Start()
    {
        UpdateState();
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.miscEvents.OnDialogueEnded += OnDialogueEnded;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.miscEvents.OnDialogueEnded -= OnDialogueEnded;
    }

    private void OnDialogueEnded(DialogueTrigger trigger)
    {
        if (trigger.name.Contains("BoothGuy"))
        {
            QuestManager.Instance.ChangeQuestState(questId, QuestState.CAN_FINISH);
        }
    }

    private void UpdateState()
    {
        string state = "Talk to the phone booth guy";
        string status = "Talk to the phone booth guy";
        ChangeState(state, status);
    }

    protected override void SetQuestStepState(string state)
    {
        UpdateState();
    }
}