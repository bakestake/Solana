using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarOfTheDeadQuestStep1 : QuestStep
{
    public GameObject yeetioPrefab;
    public Transform yeetioSpawnPoint;
    public Dialogue yeetioCaveEntranceDialogue;
    private GameObject yeetioInstance;
    private bool hasInteracted = false;
    private bool isInitialized = false;

    private void Start()
    {
        if (isInitialized)
        {
            UpdateState();
            SpawnYeetio();
        }
    }

    public override void Initialize(string questId, int stepIndex)
    {
        base.Initialize(questId, stepIndex);
        isInitialized = true;
        UpdateState();
        SpawnYeetio();
    }

    private void SpawnYeetio()
    {
        if (yeetioSpawnPoint != null && yeetioPrefab != null && yeetioInstance == null)
        {
            yeetioInstance = Instantiate(yeetioPrefab, yeetioSpawnPoint.position, Quaternion.identity);

            DialogueTrigger trigger = yeetioInstance.GetComponent<DialogueTrigger>();
            if (trigger != null)
            {
                trigger.gameObject.name = "YeetioCaveEntrance";
                trigger.defaultDialogue = yeetioCaveEntranceDialogue;
            }
        }
    }

    private void OnEnable()
    {
        GameEventsManager.instance.miscEvents.onDialogueEnded += OnDialogueEnded;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.miscEvents.onDialogueEnded -= OnDialogueEnded;
    }

    private void OnDialogueEnded(DialogueTrigger trigger)
    {
        if (!hasInteracted && trigger != null)
        {
            if (trigger.gameObject.name == "YeetioCaveEntrance" &&
                trigger.defaultDialogue.actors[0].Name.Contains("Yeetio"))
            {
                hasInteracted = true;
                Destroy(trigger.gameObject);
                FinishQuestStep();
            }
        }
    }

    private void UpdateState()
    {
        if (!isInitialized)
        {
            return; // Don't update state until initialized
        }

        string state = hasInteracted ? "completed" : "ongoing";
        string status = "Meet with Yeetio outside the cave entrance.";
        ChangeState(state, status);
    }

    protected override void SetQuestStepState(string state)
    {
        hasInteracted = state == "completed";

        if (!hasInteracted)
        {
            SpawnYeetio();
        }
    }
}
