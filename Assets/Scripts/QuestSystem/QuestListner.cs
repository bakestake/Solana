using UnityEngine;
using UnityEngine.Events;

public class QuestListner : QuestChecker
{
    [SerializeField] private bool triggerOnce;
    [SerializeField] private UnityEvent OnTriggered;

    private void OnEnable()
    {
        GameEventsManager.Instance.questEvents.onQuestStateChange += QuestStateChange;
    }

    private void OnDisable()
    {
        GameEventsManager.Instance.questEvents.onQuestStateChange -= QuestStateChange;
    }

    private void QuestStateChange(Quest quest)
    {
        if (questInfoSO == quest.info && CheckQuest())
        {
            OnTriggered?.Invoke();
            if (triggerOnce)
            {
                enabled = false;
            }
        }
    }
}