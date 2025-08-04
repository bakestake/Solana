using UnityEngine;

public class QuestStateController : MonoBehaviour
{
    [SerializeField] private QuestInfoSO questData;

    public string QuestId { get; private set; }

    public void StartQuest()
    {
        GameEventsManager.Instance.questEvents.StartQuest(QuestId);
    }

    public void FinishQuest()
    {
        GameEventsManager.Instance.questEvents.FinishQuest(QuestId);
    }

    public void SetInProgress()
    {
        Quest quest = QuestManager.Instance.GetQuestById(QuestId);
        quest.state = QuestState.IN_PROGRESS;
        GameEventsManager.Instance.questEvents.QuestStateChange(quest);
    }

    public void SetCanFinish()
    {
        Quest quest = QuestManager.Instance.GetQuestById(QuestId);
        quest.state = QuestState.CAN_FINISH;
        GameEventsManager.Instance.questEvents.QuestStateChange(quest);
    }

    public void SetFinished()
    {
        Quest quest = QuestManager.Instance.GetQuestById(QuestId);
        quest.state = QuestState.FINISHED;
        GameEventsManager.Instance.questEvents.QuestStateChange(quest);
    }
}
