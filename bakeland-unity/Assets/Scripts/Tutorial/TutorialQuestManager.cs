using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialQuestManager : MonoBehaviour
{
    public TutorialQuest activeQuest;
    public TutorialPoiPointer tutorialPoiPointer;
    public TextMeshProUGUI questLogText;

    public void StartQuest(TutorialQuest quest)
    {
        activeQuest = quest;
        EnableActivePoi();
        Debug.Log($"Quest '{quest.questName}' started!");
    }

    public void ReachPOI(TutorialPOI poi)
    {
        if (activeQuest != null && !activeQuest.IsCompleted())
        {
            TutorialPOI currentPOI = activeQuest.GetCurrentPOI();
            if (currentPOI == poi)
            {
                poi.TriggerEvent();
                poi.visited = true;
                PlayerController.canMove = false;
            }
            else
            {
                Debug.Log($"You must visit {currentPOI.poiName} first.");
            }
        }
    }

    public void NextStep()
    {
        PlayerController.canMove = true;
        Debug.Log("Can Move true");
        activeQuest.ProgressToNextStep();
        EnableActivePoi();
    }

    public bool IsQuestActive()
    {
        return activeQuest != null && !activeQuest.IsCompleted();
    }

    private void EnableActivePoi()
    {
        for (int i = 0; i < activeQuest.poiList.Count; i++)
        {
            if (activeQuest.currentStep != i)
            {
                activeQuest.poiList[i].gameObject.SetActive(false);
            }
        }

        activeQuest.poiList[activeQuest.currentStep].gameObject.SetActive(true);
        questLogText.text = activeQuest.poiList[activeQuest.currentStep].questLogText;
        tutorialPoiPointer.currentPOI = activeQuest.poiList[activeQuest.currentStep].transform;
    }
}