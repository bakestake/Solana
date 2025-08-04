using UnityEngine;

public class GoToWormholeQuestStep : QuestStep
{
    private int internalStep;
    public int CurrentStep => internalStep;

    public void SetState(int state)
    {
        internalStep = state;
        SetQuestStepState(state.ToString());
    }

    public void Complete()
    {
        FinishQuestStep();
    }

    protected override void SetQuestStepState(string state)
    {
        if (int.TryParse(state, out int result))
        {
            internalStep = result;
        }
        UpdateState();
    }

    private void UpdateState()
    {
        string status;
        if (internalStep == 0)
        {
            status = "Take the bus to Wormhole HQ.";
        }
        else if (internalStep == 1)
        {
            status = "Enter Wormhole HQ.";
        }
        else if (internalStep == 2)
        {
            status = "Enter the main chamber of Wormhole HQ.";
        }
        else
        {
            status = "Enter the main chamber of Wormhole HQ. (Completed)";
        }

        string state = internalStep >= 3 ? "completed" : "ongoing";
        if (internalStep >= 4)
        {
            FinishQuestStep();
        }

        ChangeState(state, status);
    }
}
