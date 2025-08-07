using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestChecker : MonoBehaviour
{
    public QuestInfoSO questInfoSO;
    public List<QuestState> questStates;
    public QuestManager questManager;

    private void Start()
    {
        questManager = QuestManager.instance;
    }

    public bool CheckQuest()
    {
        return questManager.CheckQuestStartedById(questInfoSO.id, questStates);
    }
}
