using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestInteractionCondition : IInteractionCondition
{
    [SerializeField] private QuestInfoSO quest;
    [SerializeField] private List<QuestState> states = new List<QuestState>() { QuestState.FINISHED, QuestState.CAN_FINISH, QuestState.IN_PROGRESS };

    public bool CanInteract()
    {
        return QuestManager.Instance.CheckQuestStartedById(quest.id, states);
    }

    public void OnInteracted()
    {

    }
}
