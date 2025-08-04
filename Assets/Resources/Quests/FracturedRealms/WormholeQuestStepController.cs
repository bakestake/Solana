using UnityEngine;

public class WormholeQuestStepController : MonoBehaviour
{
    private GoToWormholeQuestStep wormholeQuestStep;
    private bool isInitialized;

    public void SetStep(int step)
    {
        if (!TryInitialize()) return;
        wormholeQuestStep.SetState(step);
    }

    public void NextStep()
    {
        if (!TryInitialize()) return;
        int nextStep = wormholeQuestStep.CurrentStep + 1;
        wormholeQuestStep.SetState(nextStep);
    }

    public void ResetStep()
    {
        if (!TryInitialize()) return;
        wormholeQuestStep.SetState(0);
    }

    public void Complete()
    {
        if (!TryInitialize()) return;
        wormholeQuestStep.Complete();
    }

    private bool TryInitialize()
    {
        if (isInitialized) return true;
        wormholeQuestStep = FindFirstObjectByType<GoToWormholeQuestStep>();
        isInitialized = wormholeQuestStep != null;
        return isInitialized;
    }
}