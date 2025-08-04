using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialQuest
{
    public string questName;
    public List<TutorialPOI> poiList;
    public int currentStep = 0;
    public UnityEvent endEvent;

    // public TutorialQuest(string name, List<TutorialPOI> pois)
    // {
    //     questName = name;
    //     poiList = pois;
    // }

    public TutorialQuest(string name, List<TutorialPOI> pois, UnityEvent completeEvent)
    {
        questName = name;
        poiList = pois;
        endEvent = completeEvent;
    }

    public TutorialPOI GetCurrentPOI()
    {
        return poiList[currentStep];
    }

    public void ProgressToNextStep()
    {
        if (currentStep < poiList.Count - 1)
        {
            currentStep++;
        }
        else
        {
            endEvent?.Invoke();
        }
    }

    public bool IsCompleted()
    {
        return currentStep >= poiList.Count;
    }
}