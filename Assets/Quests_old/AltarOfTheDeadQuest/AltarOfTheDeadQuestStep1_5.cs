using UnityEngine;
using UnityEngine.SceneManagement;

public class AltarOfTheDeadQuestStep1_5 : QuestStep
{
    private GameObject yeetio;
    private bool isYeetioInsideCave = true;

    private void Start()
    {
        UpdateState();
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.miscEvents.OnDialogueEnded += OnDialogueEnded;
        SceneManager.sceneLoaded += (x, y) => OnSceneLoaded(x);
        SceneManager.sceneUnloaded += (x) => OnSceneUnloaded(x);
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.miscEvents.OnDialogueEnded -= OnDialogueEnded;
        SceneManager.sceneLoaded -= (x, y) => OnSceneLoaded(x);
        SceneManager.sceneUnloaded -= (x) => OnSceneUnloaded(x);
    }

    private void OnDialogueEnded(DialogueTrigger trigger)
    {
        if (isYeetioInsideCave && trigger.DefaultDialogue.actors[0].Name.Contains("Yeetio")
        )
        {
            LoadingWheel.instance.FadeInOut(MoveYeetio);
        }
    }

    private void MoveYeetio()
    {
        isYeetioInsideCave = false;
        yeetio.SetActive(false);
        FinishQuestStep();
    }

    private void OnSceneLoaded(Scene scene)
    {
        if (scene.name.Contains("Cave"))
        {
            foreach (var obj in scene.GetRootGameObjects())
            {
                if (obj.CompareTag("NPC") && obj.name.Contains("Yeetio"))
                {
                    yeetio = obj;
                    obj.SetActive(isYeetioInsideCave);
                }
            }
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (scene.name.Contains("Cave"))
        {
            yeetio = null;
        }
    }

    private void UpdateState()
    {
        string state = !isYeetioInsideCave ? "completed" : "ongoing";
        string status = "Talk to Yeetio inside the cave.";
        ChangeState(state, status);
    }

    protected override void SetQuestStepState(string state)
    {
        // isYeetioInsideCave = state == "ongoing";
    }
}
