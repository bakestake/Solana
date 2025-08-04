using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
    protected string questId;
    protected int stepIndex;
    public bool IsFinished { get; private set; }

    public virtual void Initialize(string questId, int stepIndex)
    {
        this.questId = questId;
        this.stepIndex = stepIndex;
        Debug.Log($"Initialized quest step for quest {questId} at step {stepIndex}");
    }

    public void InitializeQuestStep(string questId, int stepIndex, string questStepState)
    {
        this.questId = questId;
        this.stepIndex = stepIndex;
        if (questStepState != null && questStepState != "")
        {
            SetQuestStepState(questStepState);
        }
    }

    protected void FinishQuestStep()
    {
        Debug.Log($"Finishing quest step for quest {questId} at step {stepIndex}");
        GameEventsManager.Instance.questEvents.AdvanceQuest(questId);
    }

    protected void FinishQuest()
    {
        GameEventsManager.Instance.questEvents.FinishQuest(questId);
    }

    protected void ChangeState(string state, string status)
    {
        QuestStepState questStepState = new QuestStepState();
        questStepState.state = state;
        questStepState.status = status;

        GameEventsManager.Instance.questEvents.QuestStepStateChange(questId, stepIndex, questStepState);
        Debug.Log($"Changed state for quest {questId} step {stepIndex} to {state}");
    }

    protected abstract void SetQuestStepState(string state);

    public void LoadState(QuestStepState state)
    {
        if (state != null)
        {
            SetQuestStepState(state.state);
            ChangeState(state.state, state.status);
        }
    }

    public string QuestId => questId;
}
