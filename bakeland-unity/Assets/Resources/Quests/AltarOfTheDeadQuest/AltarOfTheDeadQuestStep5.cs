using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltarOfTheDeadQuestStep5 : QuestStep
{
    public GameObject bozitoPrefab;
    public Transform bozitoSpawnPoint;
    private GameObject bozitoInstance;
    private bool confrontationComplete = false;

    private void Start()
    {
        UpdateState();
        SpawnBozito();
    }

    private void SpawnBozito()
    {
        if (bozitoSpawnPoint != null && bozitoPrefab != null)
        {
            bozitoInstance = Instantiate(bozitoPrefab, bozitoSpawnPoint.position, Quaternion.identity);
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
        if (!confrontationComplete &&
            trigger.GetComponent<DialogueTrigger>()?.defaultDialogue.actors[0].Name.Contains("Bozito") == true)
        {
            confrontationComplete = true;
            FinishQuestStep();
        }
    }

    private void UpdateState()
    {
        string state = "";
        string status = "Confront Bozito.";
        ChangeState(state, status);
    }

    protected override void SetQuestStepState(string state)
    {
        // Not needed for this step
    }
}
