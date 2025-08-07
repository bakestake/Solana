using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialInitializer : MonoBehaviour
{
    public List<TutorialPOI> tutorialPOIs;
    public UnityEvent endEvent;
    public GameObject tutorialSequence;
    public GameObject questLogScreen;

    private void Start()
    {
        tutorialSequence.SetActive(false);
        questLogScreen.SetActive(false);
    }

    public void StartQuest()
    {
        tutorialSequence.SetActive(true);
        questLogScreen.SetActive(true);

        TutorialQuest tutorialQuest = new TutorialQuest("Tutorial Quest", tutorialPOIs, endEvent);

        TutorialQuestManager questManager = FindObjectOfType<TutorialQuestManager>();
        questManager.StartQuest(tutorialQuest);

        Debug.Log("Started quest with first poi:" + tutorialQuest.GetCurrentPOI().name);
    }
}
