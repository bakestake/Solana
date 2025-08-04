using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExploreTheCaveQuestStep2 : QuestStep
{
    private void Start()
    {
        UpdateState();
    }

    private void OnEnable()
    {
        GameEventsManager.Instance.miscEvents.OnDialogueEnded += Interacted;
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.miscEvents.OnDialogueEnded -= Interacted;
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    private void Interacted(DialogueTrigger go)
    {
        if (go.GetComponent<DialogueTrigger>() != null)
        {
            if (go.GetComponent<DialogueTrigger>().DefaultDialogue.actors[0].Name.Contains("Bozito"))
            {
                FinishQuestStep();
            }
        }
    }

    private void SceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        if (sceneMode == LoadSceneMode.Additive && scene.name.Contains("Cave"))
        {
            var bozito = scene.GetRootGameObjects().FirstOrDefault(x => x.name.Contains("Bozito"));
            if (bozito != null) bozito.SetActive(true);
            else Debug.LogError($"{name}:could not find Bozito!");
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
