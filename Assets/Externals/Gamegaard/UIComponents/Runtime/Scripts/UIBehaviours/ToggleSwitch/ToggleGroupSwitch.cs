using Gamegaard;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleGroupSwitch : MonoBehaviour
{
    [SerializeField] private Transform toggleArea;
    [SerializeField] private StepTracker stepTracker;

    private readonly List<Toggle> togglesList = new List<Toggle>();

    private void Awake()
    {
        Toggle[] toggles = toggleArea.GetComponentsInChildren<Toggle>();
        togglesList.AddRange(toggles);
        stepTracker.SetAmount(togglesList.Count);
        stepTracker.OnStepChanged += OnStepChanged;
    }

    private void OnStepChanged(int index)
    {
        togglesList[index].isOn = true;
    }

    public void NextStep()
    {
        stepTracker.NextStep();
    }

    public void PreviousStep()
    {
        stepTracker.PreviousStep();
    }
}