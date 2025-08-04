using Gamegaard;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionSelector : Selectable
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI optionText;

    [Header("Steps")]
    [SerializeField] private StepTracker step;

    [Header("Options")]
    [SerializeField] private string[] options;

    protected override void Start()
    {
        step.OnStepChanged += UpdateStepInfo;
        step.SetAmount(options.Length);
        step.SetStep(0);
    }

    public void NextStep()
    {
        step.NextStep();
    }

    public void PreviousStep()
    {
        step.PreviousStep();
    }

    private void UpdateStepInfo(int index)
    {
        optionText.text = options[index];
    }
}