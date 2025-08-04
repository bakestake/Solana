using Bakeland;
using UnityEngine;

public class AltarOfTheDeadQuestStep5 : QuestStep
{
    public Dialogue bozitoDialogue;
    private GameObject bozitoInstance;
    private bool confrontationComplete = false;

    private void Start()
    {
        UpdateState();
        SpawnBozito();
    }

    private void SpawnBozito()
    {
        bozitoInstance = NPCManager.GetNPC("Bozito");
        bozitoInstance.SetActive(true);
        bozitoInstance.GetComponent<DialogueTrigger>().LoadDialogue(bozitoDialogue);
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
        if (!confrontationComplete &&
            trigger.Actors[0].Name.Contains("Bozito") == true)
        {
            LoadingWheel.instance.FadeInOut(HideBozito);
        }
    }

    private void HideBozito()
    {
        confrontationComplete = true;
        bozitoInstance.SetActive(false);
        FinishQuest();
    }

    private void UpdateState()
    {
        string state = "";
        string status = "Confront Bozito outside.";
        ChangeState(state, status);
    }

    protected override void SetQuestStepState(string state)
    {
        // Not needed for this step
    }
}
