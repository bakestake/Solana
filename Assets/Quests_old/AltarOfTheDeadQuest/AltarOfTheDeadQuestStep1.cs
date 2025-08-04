using Bakeland;
using System.Collections;
using UnityEngine;

public class AltarOfTheDeadQuestStep1 : QuestStep
{
    public Transform yeetioSpawnPoint;
    public Dialogue yeetioCaveEntranceDialogue;
    private bool hasInteracted;

    private void Start()
    {
        UpdateState();
        // MoveYeetio();
        MoveYeetioAfterFade();
    }

    private void MoveYeetioAfterFade()
    {
        LoadingWheel.instance.FadeInOut(MoveYeetio);
    }

    private void MoveYeetio()
    {
        var yeetio = NPCManager.GetNPC("Yeetio");

        Debug.Log($"{name}:moving Yeetio to the cave entrance");
        yeetio.transform.position = yeetioSpawnPoint.position;
        if (yeetio.TryGetComponent(out DialogueTrigger dialogueTrigger))
        {
            dialogueTrigger.LoadDialogue(yeetioCaveEntranceDialogue);
        }
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
        Debug.Log(trigger.Actors[0].Name);

        if (trigger.Actors[0].Name.Contains("Yeetio"))
        {
            LoadingWheel.instance.FadeInOut(HideYeetio);
        }
    }

    private void HideYeetio()
    {
        NPCManager.GetNPC("Yeetio").SetActive(false);
        hasInteracted = true;
        FinishQuestStep();
    }

    private void UpdateState()
    {
        string state = hasInteracted ? "completed" : "ongoing";
        string status = "Meet with Yeetio outside the cave entrance.";
        ChangeState(state, status);
    }

    protected override void SetQuestStepState(string state)
    {
        hasInteracted = state == "completed";
    }
}
