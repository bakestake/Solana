using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class QuestPoint : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestInfoSO questInfoForPoint;

    [Header("Config")]
    [SerializeField] private bool startPoint = true;
    [SerializeField] private bool finishPoint = true;

    private bool playerIsNear = false;
    private string questId;
    private QuestState currentQuestState;

    private QuestIcon questIcon;

    private void Awake()
    {
        questId = questInfoForPoint.id;
        questIcon = GetComponentInChildren<QuestIcon>();

    }

    private void OnEnable()
    {
        GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
        // GameEventsManager.instance.miscEvents.onInteracted += Interacted;
        GameEventsManager.instance.miscEvents.onDialogueEnded += DialogueEnded;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
        // GameEventsManager.instance.miscEvents.onInteracted -= Interacted;
        GameEventsManager.instance.miscEvents.onDialogueEnded -= DialogueEnded;
    }

    private void Start()
    {
        if (GetComponent<DialogueTrigger>() != null)
        {
            GetComponent<DialogueTrigger>().LoadDialogue(questInfoForPoint.questStartDialogue);
        }
    }

    private void Interacted(Interact interactGo)
    {
        if (!playerIsNear)
        {
            return;
        }

        // start or finish a quest
        if (currentQuestState.Equals(QuestState.CAN_START) && startPoint)
        {
            GameEventsManager.instance.questEvents.StartQuest(questId);
        }
        else if (currentQuestState.Equals(QuestState.CAN_FINISH) && finishPoint)
        {
            GameEventsManager.instance.questEvents.FinishQuest(questId);
        }
    }

    private void DialogueEnded(DialogueTrigger trigger)
    {
        if (!string.IsNullOrEmpty(questId) && startPoint &&
            currentQuestState == QuestState.CAN_START &&
            trigger.gameObject == this.gameObject)
        {
            GameEventsManager.instance.questEvents.StartQuest(questId);
        }

        if (!string.IsNullOrEmpty(questId) && finishPoint &&
            currentQuestState == QuestState.CAN_FINISH &&
            trigger.gameObject == this.gameObject)
        {
            GameEventsManager.instance.questEvents.FinishQuest(questId);
        }
    }

    private void QuestStateChange(Quest quest)
    {
        // only update the quest state if this point has the corresponding quest
        if (quest.info.id.Equals(questId))
        {
            currentQuestState = quest.state;
            questIcon.SetState(currentQuestState, startPoint, finishPoint);
            if (currentQuestState.Equals(QuestState.CAN_START) && startPoint)
            {
                if (GetComponent<DialogueTrigger>() != null)
                {
                    GetComponent<DialogueTrigger>().LoadDialogue(questInfoForPoint.questStartDialogue);
                }
            }
            else if (currentQuestState.Equals(QuestState.IN_PROGRESS) && startPoint)
            {
                if (GetComponent<DialogueTrigger>() != null)
                {
                    GetComponent<DialogueTrigger>().LoadDialogue(questInfoForPoint.questProgressDialogue);
                }
            }
            else if (currentQuestState.Equals(QuestState.CAN_FINISH) && finishPoint)
            {
                if (GetComponent<DialogueTrigger>() != null)
                {
                    GetComponent<DialogueTrigger>().LoadDialogue(questInfoForPoint.questFinishDialogue);
                }
            }
            else if (currentQuestState.Equals(QuestState.FINISHED) && finishPoint)
            {
                if (GetComponent<DialogueTrigger>() != null)
                {
                    GetComponent<DialogueTrigger>().ResetDialogue();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.CompareTag("Player"))
        {
            playerIsNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D otherCollider)
    {
        if (otherCollider.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }
}
