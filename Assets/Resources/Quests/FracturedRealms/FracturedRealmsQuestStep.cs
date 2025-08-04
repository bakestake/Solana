using UnityEngine;

public class FracturedRealmsQuestStep : QuestStep
{
    [SerializeField] private int targetRelicAmount = 6;

    private int currentRelicAmount = 0;

    public void IncreaseRelicAmount()
    {
        currentRelicAmount++;
        UpdateState();
    }

    public void SetRelicAmount(int amount)
    {
        currentRelicAmount = amount;
        UpdateState();
    }

    public void DecreaseRelicAmount()
    {
        currentRelicAmount--;
        UpdateState();
    }

    public void ResetRelicAmount()
    {
        currentRelicAmount = 0;
        UpdateState();
    }

    protected override void SetQuestStepState(string state)
    {
        if (int.TryParse(state, out int result))
        {
            currentRelicAmount = result;
        }
        UpdateState();
    }

    private void UpdateState()
    {
        string state = currentRelicAmount.ToString();
        string status = $"Collected All Relics. [{currentRelicAmount}/{targetRelicAmount}]";
        ChangeState(state, status);

        if(currentRelicAmount >= targetRelicAmount)
        {
            FinishQuestStep();
        }
    }
}