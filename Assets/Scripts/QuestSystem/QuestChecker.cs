using System.Collections.Generic;
using UnityEngine;

public class QuestChecker : MonoBehaviour
{
    [SerializeField] protected QuestInfoSO questInfoSO;
    [SerializeField] protected List<QuestState> questStates;
    
    private QuestManager questManager;

    private void Start()
    {
        questManager = QuestManager.Instance;
    }

    public bool CheckQuest()
    {
        return questManager.CheckQuestStartedById(questInfoSO.id, questStates);
    }
}
