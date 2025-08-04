using UnityEngine;

public class FracturedRealmsStepController : MonoBehaviour
{
    private FracturedRealmsQuestStep fracturedRealmsQuestStep;

    private void Awake()
    {
        fracturedRealmsQuestStep = FindFirstObjectByType<FracturedRealmsQuestStep>();
    }

    public void IncreaseRelicAmount()
    {
        if (fracturedRealmsQuestStep == null) return;
        fracturedRealmsQuestStep.IncreaseRelicAmount();
    }

    public void SetRelicAmount(int amount)
    {
        if (fracturedRealmsQuestStep == null) return;
        fracturedRealmsQuestStep.SetRelicAmount(amount);
    }

    public void DecreaseRelicAmount()
    {
        if (fracturedRealmsQuestStep == null) return;
        fracturedRealmsQuestStep.DecreaseRelicAmount();
    }

    public void ResetRelicAmount()
    {
        if (fracturedRealmsQuestStep == null) return;
        fracturedRealmsQuestStep.ResetRelicAmount();
    }
}